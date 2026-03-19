using AccountService.Query.Domain;
using AccountService.Query.Infrastructure;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Mapping;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Text.Json;

namespace AccountService.Query.Application.Consumers;

/// <summary>
/// Event handler for AccountCreatedEvent.
/// </summary>
public class AccountEventConsumer : IEventHandler
{
    private readonly ReadDbContext _readDb;
    private readonly ILogger<AccountEventConsumer> _logger;

    public string EventType => "AccountService.Command.Domain.Events.AccountCreatedEvent";

    public AccountEventConsumer(ReadDbContext readDb, ILogger<AccountEventConsumer> logger)
    {
        _readDb = readDb;
        _logger = logger;
    }

    public async Task HandleAsync(JsonElement payload)
    {
        if (payload.TryGetProperty("DeletedAt", out _))
        {
            await HandleAccountDeletedAsync(payload);
            return;
        }

        if (payload.TryGetProperty("FirstName", out _) && payload.TryGetProperty("CreatedAt", out _))
        {
            await HandleAccountCreatedAsync(payload);
            return;
        }

        if (payload.TryGetProperty("FirstName", out _))
        {
            await HandleAccountUpdatedAsync(payload);
            return;
        }

        if (payload.TryGetProperty("Role", out _))
        {
            await HandleAccountRoleUpdatedAsync(payload);
            return;
        }

        if (payload.TryGetProperty("UpdatedAt", out _))
        {
            await HandleAccountPasswordChangedAsync(payload);
        }
    }

    public async Task HandleAccountCreatedAsync(JsonElement payload)
    {
        try
        {
            var accountId = payload.GetProperty("AccountId").GetGuid();
            var readModel = JsonElementMapper.Map<AccountReadModel>(payload, static (source, model) =>
            {
                model.Id = source.GetRequiredGuid("AccountId");
                model.UpdatedAt = DateTime.UtcNow;
            });
            var email = readModel.Email;

            await _readDb.Accounts.ReplaceOneAsync(
                x => x.Id == accountId,
                readModel,
                new ReplaceOptions { IsUpsert = true });
            _logger.LogInformation("Account read model created for {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling AccountCreatedEvent");
        }
    }

    public async Task HandleAccountUpdatedAsync(JsonElement payload)
    {
        try
        {
            var accountId = payload.GetProperty("AccountId").GetGuid();
            var update = Builders<AccountReadModel>.Update
                .Set(x => x.Email, payload.GetProperty("Email").GetString()!)
                .Set(x => x.FirstName, payload.GetProperty("FirstName").GetString()!)
                .Set(x => x.LastName, payload.GetProperty("LastName").GetString()!)
                .Set(x => x.UpdatedAt, payload.GetProperty("UpdatedAt").GetDateTime());

            await _readDb.Accounts.UpdateOneAsync(x => x.Id == accountId, update);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling AccountUpdatedEvent");
        }
    }

    public async Task HandleAccountRoleUpdatedAsync(JsonElement payload)
    {
        try
        {
            var accountId = payload.GetProperty("AccountId").GetGuid();
            var update = Builders<AccountReadModel>.Update
                .Set(x => x.Role, payload.GetProperty("Role").GetString()!)
                .Set(x => x.UpdatedAt, payload.GetProperty("UpdatedAt").GetDateTime());

            await _readDb.Accounts.UpdateOneAsync(x => x.Id == accountId, update);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling AccountRoleUpdatedEvent");
        }
    }

    public async Task HandleAccountPasswordChangedAsync(JsonElement payload)
    {
        try
        {
            var accountId = payload.GetProperty("AccountId").GetGuid();
            var update = Builders<AccountReadModel>.Update
                .Set(x => x.UpdatedAt, payload.GetProperty("UpdatedAt").GetDateTime());

            await _readDb.Accounts.UpdateOneAsync(x => x.Id == accountId, update);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling AccountPasswordChangedEvent");
        }
    }

    public async Task HandleAccountDeletedAsync(JsonElement payload)
    {
        try
        {
            var accountId = payload.GetProperty("AccountId").GetGuid();
            await _readDb.Accounts.DeleteOneAsync(x => x.Id == accountId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling AccountDeletedEvent");
        }
    }
}
