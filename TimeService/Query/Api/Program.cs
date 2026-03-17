using Infrastructure.Api.Authentication;
using Infrastructure.Api.Middleware;
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
builder.Services.AddScoped<ITimeEntryReadRepository, TimeEntryReadRepository>();
builder.Services.AddScoped<ITimesheetReadRepository, TimesheetReadRepository>();
builder.Services.AddScoped<ILeaveRequestReadRepository, LeaveRequestReadRepository>();
builder.Services.AddScoped<ILeaveBalanceReadRepository, LeaveBalanceReadRepository>();
builder.Services.AddScoped<IReferenceDataRepository, ReferenceDataRepository>();

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
