using Infrastructure.Api.Authentication;
using Infrastructure.Api.Extensions;
using Infrastructure.Api.Middleware;
using Infrastructure.Api.Messaging;
using TimeService.Command.Application.Commands;
using TimeService.Command.Application.DTOs;
using TimeService.Command.Application.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddWorkForceHubJwtAuthentication(builder.Configuration);
builder.Services.AddWorkForceHubSwagger("WorkForceHub Time Command API");
builder.Services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
builder.Services.AddHandlersFromAssemblies(typeof(CreateTimeEntryHandler).Assembly);
builder.Services.AddSingleton<IKafkaProducer>(sp =>
{
    var bootstrapServers = builder.Configuration.GetSection("Kafka")["BootstrapServers"] ?? "localhost:29092";
    var logger = sp.GetRequiredService<ILogger<KafkaProducer>>();
    return new KafkaProducer(logger, bootstrapServers);
});
builder.Services.AddScoped<ICommandHandler<CreateTimeEntryCommand, CommandAcceptedResponse>, CreateTimeEntryHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateTimeEntryCommand, CommandAcceptedResponse>, UpdateTimeEntryHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteTimeEntryCommand, CommandAcceptedResponse>, DeleteTimeEntryHandler>();
builder.Services.AddScoped<ICommandHandler<CreateTimesheetCommand, CommandAcceptedResponse>, CreateTimesheetHandler>();
builder.Services.AddScoped<ICommandHandler<SubmitTimesheetCommand, CommandAcceptedResponse>, SubmitTimesheetHandler>();
builder.Services.AddScoped<ICommandHandler<ApproveTimesheetCommand, CommandAcceptedResponse>, ApproveTimesheetHandler>();
builder.Services.AddScoped<ICommandHandler<RejectTimesheetCommand, CommandAcceptedResponse>, RejectTimesheetHandler>();
builder.Services.AddScoped<ICommandHandler<ReopenTimesheetCommand, CommandAcceptedResponse>, ReopenTimesheetHandler>();
builder.Services.AddScoped<ICommandHandler<CreateLeaveRequestCommand, CommandAcceptedResponse>, CreateLeaveRequestHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateLeaveRequestCommand, CommandAcceptedResponse>, UpdateLeaveRequestHandler>();
builder.Services.AddScoped<ICommandHandler<SubmitLeaveRequestCommand, CommandAcceptedResponse>, SubmitLeaveRequestHandler>();
builder.Services.AddScoped<ICommandHandler<ApproveLeaveRequestCommand, CommandAcceptedResponse>, ApproveLeaveRequestHandler>();
builder.Services.AddScoped<ICommandHandler<RejectLeaveRequestCommand, CommandAcceptedResponse>, RejectLeaveRequestHandler>();
builder.Services.AddScoped<ICommandHandler<CancelLeaveRequestCommand, CommandAcceptedResponse>, CancelLeaveRequestHandler>();
builder.Services.AddScoped<ICommandHandler<AdjustLeaveBalanceCommand, CommandAcceptedResponse>, AdjustLeaveBalanceHandler>();
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();

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
