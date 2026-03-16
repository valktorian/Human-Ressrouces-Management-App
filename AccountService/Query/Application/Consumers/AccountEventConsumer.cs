using AccountService.Query.Domain;
using AccountService.Query.Infrastructure;
using Infrastructure.Api.Messaging;
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
        await HandleAccountCreatedAsync(payload);
    }

    public async Task HandleAccountCreatedAsync(JsonElement payload)
    {
        try
        {
            var accountId = payload.GetProperty("AccountId").GetGuid();
            var email = payload.GetProperty("Email").GetString()!;
            var firstName = payload.GetProperty("FirstName").GetString()!;
            var lastName = payload.GetProperty("LastName").GetString()!;
            var role = payload.GetProperty("Role").GetString()!;
            var isActive = payload.GetProperty("IsActive").GetBoolean();
            var createdAt = payload.GetProperty("CreatedAt").GetDateTime();

            var readModel = new AccountReadModel
            {
                Id = accountId,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Role = role,
                IsActive = isActive,
                CreatedAt = createdAt,
                UpdatedAt = DateTime.UtcNow
            };

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
}
