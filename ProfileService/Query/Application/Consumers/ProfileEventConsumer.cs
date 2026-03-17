using Infrastructure.Api.Messaging;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ProfileService.Query.Domain;
using ProfileService.Query.Infrastructure;
using System.Text.Json;

namespace ProfileService.Query.Application.Consumers;

public class ProfileEventConsumer : IEventHandler
{
    private readonly ReadDbContext _readDb;
    private readonly ILogger<ProfileEventConsumer> _logger;

    public string EventType => "ProfileService.Command.Domain.Events.ProfileCreatedEvent";

    public ProfileEventConsumer(ReadDbContext readDb, ILogger<ProfileEventConsumer> logger)
    {
        _readDb = readDb;
        _logger = logger;
    }

    public async Task HandleAsync(JsonElement payload)
    {
        if (payload.TryGetProperty("DeletedAt", out _))
        {
            await HandleDeletedAsync(payload);
            return;
        }

        await UpsertAsync(payload);
    }

    private async Task UpsertAsync(JsonElement payload)
    {
        try
        {
            var model = new ProfileReadModel
            {
                Id = payload.GetProperty("ProfileId").GetGuid(),
                AccountId = payload.TryGetProperty("AccountId", out var accountId) && accountId.ValueKind != JsonValueKind.Null ? accountId.GetGuid() : null,
                EmployeeNumber = payload.GetProperty("EmployeeNumber").GetString() ?? string.Empty,
                FirstName = payload.GetProperty("FirstName").GetString() ?? string.Empty,
                LastName = payload.GetProperty("LastName").GetString() ?? string.Empty,
                WorkEmail = payload.GetProperty("WorkEmail").GetString() ?? string.Empty,
                PersonalEmail = payload.TryGetProperty("PersonalEmail", out var personalEmail) && personalEmail.ValueKind != JsonValueKind.Null ? personalEmail.GetString() : null,
                PhoneNumber = payload.TryGetProperty("PhoneNumber", out var phone) && phone.ValueKind != JsonValueKind.Null ? phone.GetString() : null,
                Address = payload.TryGetProperty("Address", out var address) && address.ValueKind != JsonValueKind.Null ? address.GetString() : null,
                DateOfBirth = payload.TryGetProperty("DateOfBirth", out var dateOfBirth) && dateOfBirth.ValueKind != JsonValueKind.Null ? dateOfBirth.GetDateTime() : null,
                JobTitle = payload.GetProperty("JobTitle").GetString() ?? string.Empty,
                Department = payload.GetProperty("Department").GetString() ?? string.Empty,
                ManagerProfileId = payload.TryGetProperty("ManagerProfileId", out var managerId) && managerId.ValueKind != JsonValueKind.Null ? managerId.GetGuid() : null,
                EmploymentType = payload.GetProperty("EmploymentType").GetString() ?? string.Empty,
                HireDate = payload.GetProperty("HireDate").GetDateTime(),
                OrganizationRole = payload.GetProperty("OrganizationRole").GetString() ?? string.Empty,
                EmploymentStatus = payload.GetProperty("EmploymentStatus").GetString() ?? string.Empty,
                CreatedAt = payload.GetProperty("CreatedAt").GetDateTime(),
                UpdatedAt = payload.GetProperty("UpdatedAt").GetDateTime()
            };

            await _readDb.Profiles.ReplaceOneAsync(x => x.Id == model.Id, model, new ReplaceOptions { IsUpsert = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling profile upsert event.");
        }
    }

    private async Task HandleDeletedAsync(JsonElement payload)
    {
        try
        {
            var profileId = payload.GetProperty("ProfileId").GetGuid();
            await _readDb.Profiles.DeleteOneAsync(x => x.Id == profileId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling profile delete event.");
        }
    }
}
