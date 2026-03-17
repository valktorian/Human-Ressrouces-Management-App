using Infrastructure.Api;
using Infrastructure.Api.Base;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var kafkaSection = configuration.GetSection("Kafka");

var bootstrapServers = kafkaSection["BootstrapServers"] ?? "localhost:9092";
var topic = kafkaSection["Topic"] ?? "workforcehub-events";
var groupId = kafkaSection["GroupId"] ?? "workforcehub-consumer-group";

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSingleton<IKafkaProducer>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<KafkaProducer>>();
    return new KafkaProducer(logger, bootstrapServers);
});

builder.Services.AddHostedService(sp =>
    new KafkaConsumer(
        bootstrapServers,
        topic,
        groupId,
        sp.GetRequiredService<ILogger<KafkaConsumer>>()
    )
);

var app = builder.Build();

app.UseGlobalErrorHandler();

app.MapGet("/", () => Results.Ok("Infrastructure.Api is running"));

app.MapPost("/produce", async (IKafkaProducer producer) =>
{
    var evt = new TestEvent();
    var payload = new { Message = "Test message", SentAt = DateTime.UtcNow };
    await producer.ProduceAsync(evt, payload, topic);
    return Results.Ok("Event sent");
});

app.Run();

public class TestEvent : BaseEvent
{ }
