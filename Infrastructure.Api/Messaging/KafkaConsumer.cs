using Confluent.Kafka;
using System.Text.Json;

namespace Infrastructure.Api.Messaging;

public class KafkaConsumer : BackgroundService
{
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly string _topic;
    private readonly IConsumer<string, string> _consumer;
    private readonly Dictionary<string, Func<JsonElement, Task>> _eventHandlers;

    public KafkaConsumer(string bootstrapServers, string topic, string groupId, ILogger<KafkaConsumer> logger)
    {
        _logger = logger;
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
            // Let the host finish starting before entering the blocking consume loop.
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

    private async Task ConsumeEventsAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ConsumeEventsAsync started");
        try
        {
            _logger.LogInformation("Kafka consumer listening on '{Topic}'", _topic);
            try
            {
                _consumer.Subscribe(_topic);
                _logger.LogInformation("Successfully subscribed to topic '{Topic}'", _topic);
            }
            catch (Exception subEx)
            {
                _logger.LogError(subEx, "Failed to subscribe to topic '{Topic}'", _topic);
                return; // Don't throw, let app continue without consumer
            }

            _logger.LogInformation("About to enter message consumption loop");
            _logger.LogInformation("   Stopping token cancelled? {IsCancelled}", stoppingToken.IsCancellationRequested);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug(" Waiting for messages on topic '{Topic}'...", _topic);

                    ConsumeResult<string, string> result = null;
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
                        _logger.LogDebug(" Timeout waiting for message");
                        continue;
                    }

                    _logger.LogInformation("   RECEIVED MESSAGE from Kafka:");
                    _logger.LogInformation("   Raw JSON: {RawJson}", result.Message.Value);

                    var wrapper = JsonSerializer.Deserialize<EventMessage>(result.Message.Value);
                    if (wrapper == null)
                    {
                        _logger.LogWarning("Invalid event format - deserialization returned null");
                        continue;
                    }

                    _logger.LogInformation("   Deserialized event:");
                    _logger.LogInformation("   Event Type: {EventType}", wrapper.EventType);
                    _logger.LogInformation("   Event ID: {EventId}", wrapper.EventId);

                    _logger.LogInformation(" Received event {EventType}", wrapper.EventType);
                    try
                    {
                        await HandleEvent(wrapper.EventType, wrapper.Payload);
                    }
                    catch (Exception handlerEx)
                    {
                        _logger.LogError(handlerEx, "Error in event handler for {EventType}", wrapper.EventType);
                        // Don't rethrow - allow consumer to continue
                    }
                }
                catch (ConsumeException ex) when (ex.Error.Code != ErrorCode.Local_TimedOut)
                {
                    _logger.LogWarning(ex, "Kafka consume error: {Code}", ex.Error.Code);
                    await Task.Delay(1000, stoppingToken); // Backoff on error
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
            catch { }
            _logger.LogInformation("Kafka consumer stopped");
        }
    }

    private async Task HandleEvent(string eventType, JsonElement payload)
    {
        _logger.LogInformation(" Looking for handler for event type: '{EventType}'", eventType);
        _logger.LogInformation(" Registered handlers: {Handlers}", string.Join(", ", _eventHandlers.Keys));

        if (_eventHandlers.TryGetValue(eventType, out var handler))
        {
            _logger.LogInformation(" Found handler! Dispatching event '{EventType}'", eventType);
            await handler(payload);
        }
        else
        {
            _logger.LogWarning(" No handler registered for event type: {EventType}", eventType);
        }
    }
}

public class EventMessage
{
    public string EventType { get; set; }
    public JsonElement Payload { get; set; }
    public Guid EventId { get; set; }
    public DateTime OccurredAt { get; set; }
}
