using Infrastructure.Api.Authentication;
using Infrastructure.Api.Middleware;
using Infrastructure.Api.Messaging;
using TimeService.Query.Application.Consumers;
using TimeService.Query.Domain.Repositories;
using TimeService.Query.Infrastructure;
using TimeService.Query.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddWorkForceHubJwtAuthentication(builder.Configuration);
builder.Services.AddWorkForceHubSwagger("WorkForceHub Time Query API");

var readDbContext = new ReadDbContext(builder.Configuration);
builder.Services.AddSingleton(readDbContext);
builder.Services.AddScoped<TimeEventConsumer>();
builder.Services.AddScoped<ITimeEntryReadRepository, TimeEntryReadRepository>();
builder.Services.AddScoped<ITimesheetReadRepository, TimesheetReadRepository>();
builder.Services.AddScoped<ILeaveRequestReadRepository, LeaveRequestReadRepository>();
builder.Services.AddScoped<ILeaveBalanceReadRepository, LeaveBalanceReadRepository>();
builder.Services.AddScoped<IReferenceDataRepository, ReferenceDataRepository>();

var kafkaSection = builder.Configuration.GetSection("Kafka");
var bootstrap = kafkaSection["BootstrapServers"] ?? "localhost:29092";
var topic = kafkaSection["Topic"] ?? "time.events";
var group = kafkaSection["GroupId"] ?? "time-query-group";

builder.Services.AddHostedService(sp =>
{
    var logger = sp.GetRequiredService<ILogger<KafkaConsumer>>();
    var consumer = new KafkaConsumer(bootstrap, topic, group, logger);

    foreach (var eventType in new[]
    {
        "TimeService.Command.Domain.Events.TimeEntryCreatedEvent",
        "TimeService.Command.Domain.Events.TimesheetCreatedEvent",
        "TimeService.Command.Domain.Events.LeaveRequestCreatedEvent",
        "TimeService.Command.Domain.Events.TimesheetStatusChangedEvent",
        "TimeService.Command.Domain.Events.LeaveRequestStatusChangedEvent",
        "TimeService.Command.Domain.Events.LeaveBalanceAdjustedEvent"
    })
    {
        consumer.RegisterHandler(eventType, async payload =>
        {
            using var scope = sp.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<TimeEventConsumer>();
            await handler.HandleAsync(payload);
        });
    }

    return consumer;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalErrorHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
