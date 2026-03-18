using Infrastructure.Api.Messaging;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Text.Json;
using TimeService.Query.Domain;
using TimeService.Query.Infrastructure;

namespace TimeService.Query.Application.Consumers;

public class TimeEventConsumer : IEventHandler
{
    private readonly ReadDbContext _readDb;
    private readonly ILogger<TimeEventConsumer> _logger;

    public string EventType => "TimeService.Command.Domain.Events.TimeEntryCreatedEvent";

    public TimeEventConsumer(ReadDbContext readDb, ILogger<TimeEventConsumer> logger)
    {
        _readDb = readDb;
        _logger = logger;
    }

    public async Task HandleAsync(JsonElement payload)
    {
        try
        {
            if (payload.TryGetProperty("TimeEntryId", out _))
            {
                await UpsertTimeEntryAsync(payload);
                return;
            }

            if (payload.TryGetProperty("TimesheetId", out _))
            {
                if (payload.TryGetProperty("PeriodStart", out _))
                {
                    await UpsertTimesheetAsync(payload);
                }
                else
                {
                    await UpdateTimesheetStatusAsync(payload);
                }
                return;
            }

            if (payload.TryGetProperty("LeaveRequestId", out _))
            {
                if (payload.TryGetProperty("StartDate", out _))
                {
                    await UpsertLeaveRequestAsync(payload);
                }
                else
                {
                    await UpdateLeaveRequestStatusAsync(payload);
                }
                return;
            }

            if (payload.TryGetProperty("LeaveBalanceId", out _))
            {
                await UpsertLeaveBalanceAsync(payload);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling time event payload.");
        }
    }

    private async Task UpsertTimeEntryAsync(JsonElement payload)
    {
        var model = new TimeEntryReadModel
        {
            Id = payload.GetProperty("TimeEntryId").GetGuid(),
            AccountId = ReadNullableGuid(payload, "AccountId"),
            EmployeeId = payload.GetProperty("EmployeeId").GetGuid(),
            WorkDate = payload.GetProperty("WorkDate").GetDateTime(),
            StartTime = payload.GetProperty("StartTime").GetString() ?? string.Empty,
            EndTime = payload.GetProperty("EndTime").GetString() ?? string.Empty,
            Hours = payload.GetProperty("Hours").GetDecimal(),
            ProjectCode = payload.GetProperty("ProjectCode").GetString() ?? string.Empty,
            TaskCode = payload.GetProperty("TaskCode").GetString() ?? string.Empty,
            Notes = ReadNullableString(payload, "Notes"),
            Status = payload.GetProperty("Status").GetString() ?? string.Empty,
            CreatedAt = payload.GetProperty("CreatedAt").GetDateTime(),
            UpdatedAt = payload.GetProperty("UpdatedAt").GetDateTime()
        };

        await _readDb.TimeEntries.ReplaceOneAsync(x => x.Id == model.Id, model, new ReplaceOptions { IsUpsert = true });
    }

    private async Task UpsertTimesheetAsync(JsonElement payload)
    {
        var model = new TimesheetReadModel
        {
            Id = payload.GetProperty("TimesheetId").GetGuid(),
            AccountId = ReadNullableGuid(payload, "AccountId"),
            EmployeeId = payload.GetProperty("EmployeeId").GetGuid(),
            PeriodStart = payload.GetProperty("PeriodStart").GetDateTime(),
            PeriodEnd = payload.GetProperty("PeriodEnd").GetDateTime(),
            TotalHours = payload.GetProperty("TotalHours").GetDecimal(),
            Status = payload.GetProperty("Status").GetString() ?? string.Empty,
            SubmittedAt = ReadNullableDateTime(payload, "SubmittedAt"),
            ApprovedAt = ReadNullableDateTime(payload, "ApprovedAt"),
            CreatedAt = payload.GetProperty("CreatedAt").GetDateTime(),
            UpdatedAt = payload.GetProperty("UpdatedAt").GetDateTime()
        };

        await _readDb.Timesheets.ReplaceOneAsync(x => x.Id == model.Id, model, new ReplaceOptions { IsUpsert = true });
    }

    private async Task UpsertLeaveRequestAsync(JsonElement payload)
    {
        var model = new LeaveRequestReadModel
        {
            Id = payload.GetProperty("LeaveRequestId").GetGuid(),
            AccountId = ReadNullableGuid(payload, "AccountId"),
            EmployeeId = payload.GetProperty("EmployeeId").GetGuid(),
            LeaveType = payload.GetProperty("LeaveType").GetString() ?? string.Empty,
            StartDate = payload.GetProperty("StartDate").GetDateTime(),
            EndDate = payload.GetProperty("EndDate").GetDateTime(),
            Status = payload.GetProperty("Status").GetString() ?? string.Empty,
            Reason = ReadNullableString(payload, "Reason"),
            SubmittedAt = ReadNullableDateTime(payload, "SubmittedAt"),
            DecisionAt = ReadNullableDateTime(payload, "DecisionAt"),
            CreatedAt = payload.GetProperty("CreatedAt").GetDateTime(),
            UpdatedAt = payload.GetProperty("UpdatedAt").GetDateTime()
        };

        await _readDb.LeaveRequests.ReplaceOneAsync(x => x.Id == model.Id, model, new ReplaceOptions { IsUpsert = true });
    }

    private async Task UpdateTimesheetStatusAsync(JsonElement payload)
    {
        var timesheetId = payload.GetProperty("TimesheetId").GetGuid();
        var update = Builders<TimesheetReadModel>.Update
            .Set(x => x.Status, payload.GetProperty("Status").GetString() ?? string.Empty)
            .Set(x => x.UpdatedAt, payload.GetProperty("UpdatedAt").GetDateTime());

        if (payload.TryGetProperty("SubmittedAt", out var submittedAt) && submittedAt.ValueKind != JsonValueKind.Null)
        {
            update = update.Set(x => x.SubmittedAt, submittedAt.GetDateTime());
        }

        if (payload.TryGetProperty("ApprovedAt", out var approvedAt) && approvedAt.ValueKind != JsonValueKind.Null)
        {
            update = update.Set(x => x.ApprovedAt, approvedAt.GetDateTime());
        }

        await _readDb.Timesheets.UpdateOneAsync(x => x.Id == timesheetId, update);
    }

    private async Task UpdateLeaveRequestStatusAsync(JsonElement payload)
    {
        var leaveRequestId = payload.GetProperty("LeaveRequestId").GetGuid();
        var update = Builders<LeaveRequestReadModel>.Update
            .Set(x => x.Status, payload.GetProperty("Status").GetString() ?? string.Empty)
            .Set(x => x.UpdatedAt, payload.GetProperty("UpdatedAt").GetDateTime());

        if (payload.TryGetProperty("SubmittedAt", out var submittedAt) && submittedAt.ValueKind != JsonValueKind.Null)
        {
            update = update.Set(x => x.SubmittedAt, submittedAt.GetDateTime());
        }

        if (payload.TryGetProperty("DecisionAt", out var decisionAt) && decisionAt.ValueKind != JsonValueKind.Null)
        {
            update = update.Set(x => x.DecisionAt, decisionAt.GetDateTime());
        }

        await _readDb.LeaveRequests.UpdateOneAsync(x => x.Id == leaveRequestId, update);
    }

    private async Task UpsertLeaveBalanceAsync(JsonElement payload)
    {
        var leaveBalanceId = payload.GetProperty("LeaveBalanceId").GetGuid();
        var employeeId = payload.GetProperty("EmployeeId").GetGuid();
        var leaveType = payload.GetProperty("LeaveType").GetString() ?? string.Empty;
        var delta = payload.GetProperty("Delta").GetDecimal();

        var existing = await _readDb.LeaveBalances
            .Find(x => x.EmployeeId == employeeId && x.LeaveType == leaveType)
            .FirstOrDefaultAsync();

        var model = existing ?? new LeaveBalanceReadModel
        {
            Id = leaveBalanceId,
            AccountId = ReadNullableGuid(payload, "AccountId"),
            EmployeeId = employeeId,
            LeaveType = leaveType,
            Used = 0m,
            Pending = 0m
        };

        model.AccountId ??= ReadNullableGuid(payload, "AccountId");
        model.Available += delta;
        model.UpdatedAt = payload.GetProperty("UpdatedAt").GetDateTime();

        await _readDb.LeaveBalances.ReplaceOneAsync(
            x => x.EmployeeId == employeeId && x.LeaveType == leaveType,
            model,
            new ReplaceOptions { IsUpsert = true });
    }

    private static Guid? ReadNullableGuid(JsonElement payload, string propertyName)
        => payload.TryGetProperty(propertyName, out var property) && property.ValueKind != JsonValueKind.Null
            ? property.GetGuid()
            : null;

    private static DateTime? ReadNullableDateTime(JsonElement payload, string propertyName)
        => payload.TryGetProperty(propertyName, out var property) && property.ValueKind != JsonValueKind.Null
            ? property.GetDateTime()
            : null;

    private static string? ReadNullableString(JsonElement payload, string propertyName)
        => payload.TryGetProperty(propertyName, out var property) && property.ValueKind != JsonValueKind.Null
            ? property.GetString()
            : null;
}
