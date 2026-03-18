using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System.Text.Json;

namespace Infrastructure.Api.Messaging;

public class KafkaConsumer : BackgroundService
{
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly string _bootstrapServers;
    private readonly string _topic;
    private readonly IConsumer<string, string> _consumer;
    private readonly Dictionary<string, Func<JsonElement, Task>> _eventHandlers;

    public KafkaConsumer(string bootstrapServers, string topic, string groupId, ILogger<KafkaConsumer> logger)
    {
        _logger = logger;
        _bootstrapServers = bootstrapServers;
        _topic = topic;
        _eventHandlers = new Dictionary<string, Func<JsonElement, Task>>();

        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    public void RegisterHandler(string eventTypeName, Func<JsonElement, Task> handler)
    {
        _eventHandlers[eventTypeName] = handler;
        _logger.LogInformation("Registered handler for event type: {EventType}", eventTypeName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer starting");
        try
        {
            await Task.Yield();
            _logger.LogInformation("About to call ConsumeEventsAsync");
            await ConsumeEventsAsync(stoppingToken);
            _logger.LogInformation("ConsumeEventsAsync returned normally");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogInformation(ex, "Kafka consumer cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal Kafka consumer error in ExecuteAsync: {ExceptionType}", ex.GetType().Name);
            _logger.LogError(ex, "Exception message: {Message}", ex.Message);
            _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
        }
    }

    private async Task EnsureTopicExistsAsync(CancellationToken cancellationToken)
    {
        using var admin = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = _bootstrapServers
        }).Build();

        var metadata = admin.GetMetadata(TimeSpan.FromSeconds(5));
        var exists = metadata.Topics.Any(t => t.Topic == _topic && t.Error.Code != ErrorCode.UnknownTopicOrPart);

        if (exists)
        {
            return;
        }

        try
        {
            await admin.CreateTopicsAsync(new[]
            {
                new TopicSpecification
                {
                    Name = _topic,
                    NumPartitions = 1,
                    ReplicationFactor = 1
                }
            });

            _logger.LogInformation("Created missing Kafka topic '{Topic}'", _topic);
        }
        catch (CreateTopicsException ex) when (ex.Results.All(r => r.Error.Code == ErrorCode.TopicAlreadyExists))
        {
            _logger.LogInformation("Kafka topic '{Topic}' already exists", _topic);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
    }

    private async Task ConsumeEventsAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ConsumeEventsAsync started");
        try
        {
            _logger.LogInformation("Kafka consumer listening on '{Topic}'", _topic);
            try
            {
                await EnsureTopicExistsAsync(stoppingToken);
                _consumer.Subscribe(_topic);
                _logger.LogInformation("Successfully subscribed to topic '{Topic}'", _topic);
            }
            catch (Exception subEx)
            {
                _logger.LogError(subEx, "Failed to subscribe to topic '{Topic}'", _topic);
                return;
            }

            _logger.LogInformation("About to enter message consumption loop");
            _logger.LogInformation("Stopping token cancelled? {IsCancelled}", stoppingToken.IsCancellationRequested);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug("Waiting for messages on topic '{Topic}'...", _topic);

                    ConsumeResult<string, string>? result;
                    try
                    {
                        result = _consumer.Consume(TimeSpan.FromSeconds(3));
                    }
                    catch (Exception consumeEx)
                    {
                        _logger.LogError(consumeEx, "Exception in _consumer.Consume(): {ExceptionType}: {Message}", consumeEx.GetType().Name, consumeEx.Message);
                        throw;
                    }

                    if (result == null)
                    {
                        _logger.LogDebug("Timeout waiting for message");
                        continue;
                    }

                    _logger.LogInformation("RECEIVED MESSAGE from Kafka");
                    _logger.LogInformation("Raw JSON: {RawJson}", result.Message.Value);

                    var wrapper = JsonSerializer.Deserialize<EventMessage>(result.Message.Value);
                    if (wrapper == null)
                    {
                        _logger.LogWarning("Invalid event format - deserialization returned null");
                        continue;
                    }

                    _logger.LogInformation("Deserialized event type: {EventType}", wrapper.EventType);
                    _logger.LogInformation("Deserialized event id: {EventId}", wrapper.EventId);

                    try
                    {
                        await HandleEvent(wrapper.EventType, wrapper.Payload);
                    }
                    catch (Exception handlerEx)
                    {
                        _logger.LogError(handlerEx, "Error in event handler for {EventType}", wrapper.EventType);
                    }
                }
                catch (ConsumeException ex) when (ex.Error.Code == ErrorCode.UnknownTopicOrPart)
                {
                    _logger.LogWarning("Kafka topic '{Topic}' is not available yet. Retrying shortly.", _topic);
                    await EnsureTopicExistsAsync(stoppingToken);
                    await Task.Delay(1000, stoppingToken);
                }
                catch (ConsumeException ex) when (ex.Error.Code != ErrorCode.Local_TimedOut)
                {
                    _logger.LogWarning(ex, "Kafka consume error: {Code}", ex.Error.Code);
                    await Task.Delay(1000, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Kafka consumer was cancelled");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "UNEXPECTED ERROR in consumer loop: {ExceptionType}: {Message}", ex.GetType().Name, ex.Message);
                    _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
                    await Task.Delay(1000, stoppingToken);
                }
            }

            _logger.LogInformation("While loop exited normally");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogInformation(ex, "Kafka consumer shutdown - OperationCancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FATAL Kafka consumer error: {ExceptionType}: {Message}", ex.GetType().Name, ex.Message);
            _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
        }
        finally
        {
            try
            {
                _consumer.Close();
            }
            catch
            {
            }

            _logger.LogInformation("Kafka consumer stopped");
        }
    }

    private async Task HandleEvent(string eventType, JsonElement payload)
    {
        _logger.LogInformation("Looking for handler for event type: '{EventType}'", eventType);
        _logger.LogInformation("Registered handlers: {Handlers}", string.Join(", ", _eventHandlers.Keys));

        if (_eventHandlers.TryGetValue(eventType, out var handler))
        {
            _logger.LogInformation("Found handler. Dispatching event '{EventType}'", eventType);
            await handler(payload);
        }
        else
        {
            _logger.LogWarning("No handler registered for event type: {EventType}", eventType);
        }
    }
}

public class EventMessage
{
    public string EventType { get; set; } = string.Empty;
    public JsonElement Payload { get; set; }
    public Guid EventId { get; set; }
    public DateTime OccurredAt { get; set; }
}
