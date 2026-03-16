using AccountService.Query.Application.Consumers;
using AccountService.Query.Infrastructure;
using Infrastructure.Api.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var readDbContext = new ReadDbContext(builder.Configuration);
builder.Services.AddSingleton(readDbContext);
builder.Services.AddScoped<AccountEventConsumer>();

var kafkaSection = builder.Configuration.GetSection("Kafka");
var bootstrap = kafkaSection["BootstrapServers"] ?? "localhost:29092";
var topic = kafkaSection["Topic"] ?? "account.events";
var group = kafkaSection["GroupId"] ?? "account-query-group";

builder.Services.AddHostedService(sp =>
{
    var logger = sp.GetRequiredService<ILogger<KafkaConsumer>>();
    var consumer = new KafkaConsumer(bootstrap, topic, group, logger);

    consumer.RegisterHandler("AccountService.Command.Domain.Events.AccountCreatedEvent", async payload =>
    {
        using var scope = sp.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<AccountEventConsumer>();
        await handler.HandleAsync(payload);
    });

    return consumer;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
