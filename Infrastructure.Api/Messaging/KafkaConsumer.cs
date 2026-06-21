using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System.Text.Json;

namespace Infrastructure.Api.Messaging;

public class KafkaConsumer : BackgroundService
{
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly string _bootstrapServers;
    private readonly string _topic;
    private readonly string _groupId;
    private readonly IConsumer<string, string> _consumer;
    private readonly Dictionary<string, Func<JsonElement, Task>> _eventHandlers;

    public KafkaConsumer(string bootstrapServers, string topic, string groupId, ILogger<KafkaConsumer> logger)
    {
        _logger = logger;
        _bootstrapServers = bootstrapServers;
        _topic = topic;
        _groupId = groupId;
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
        _logger.LogDebug("Registered Kafka handler for event type {EventType}", eventTypeName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer starting for topic {Topic} with group {GroupId}", _topic, _groupId);
        try
        {
            await Task.Yield();
            await ConsumeEventsAsync(stoppingToken);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogDebug(ex, "Kafka consumer cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kafka consumer stopped unexpectedly for topic {Topic}", _topic);
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
            _logger.LogDebug("Kafka topic {Topic} already exists", _topic);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
    }

    private async Task ConsumeEventsAsync(CancellationToken stoppingToken)
    {
        try
        {
            await SubscribeWithRetryAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug("Waiting for messages on topic '{Topic}'...", _topic);

                    var result = _consumer.Consume(TimeSpan.FromSeconds(3));

                    if (result == null)
                    {
                        _logger.LogDebug("Timeout waiting for message");
                        continue;
                    }

                    _logger.LogDebug("Received Kafka message from {Topic} at offset {Offset}", result.Topic, result.Offset);

                    var wrapper = JsonSerializer.Deserialize<EventMessage>(result.Message.Value);
                    if (wrapper == null)
                    {
                        _logger.LogWarning("Invalid event format - deserialization returned null");
                        continue;
                    }

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
                    _logger.LogWarning("Kafka topic {Topic} is not available yet. Retrying shortly.", _topic);
                    await EnsureTopicExistsAsync(stoppingToken);
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (ConsumeException ex) when (ex.Error.Code != ErrorCode.Local_TimedOut)
                {
                    _logger.LogWarning(ex, "Kafka consume error on topic {Topic}: {Code}", _topic, ex.Error.Code);
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Kafka consumer loop error on topic {Topic}. Retrying shortly.", _topic);
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogDebug(ex, "Kafka consumer shutdown requested");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal Kafka consumer error on topic {Topic}", _topic);
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

            _logger.LogInformation("Kafka consumer stopped for topic {Topic}", _topic);
        }
    }

    private async Task SubscribeWithRetryAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await EnsureTopicExistsAsync(stoppingToken);
                _consumer.Subscribe(_topic);
                _logger.LogInformation("Kafka consumer subscribed to topic {Topic}", _topic);
                return;
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to subscribe to Kafka topic {Topic}. Retrying shortly.", _topic);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task HandleEvent(string eventType, JsonElement payload)
    {
        if (_eventHandlers.TryGetValue(eventType, out var handler))
        {
            _logger.LogDebug("Dispatching Kafka event {EventType}", eventType);
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
