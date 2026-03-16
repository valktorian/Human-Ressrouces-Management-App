using AccountService.Command.Application.Abstractions;
using AccountService.Command.Domain;
using AccountService.Command.Domain.Events;
using AccountService.Command.Infrastructure.Persistence;
using Infrastructure.Api.Messaging;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Command.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AccountCommandDbContext _dbContext;

    public AccountRepository(AccountCommandDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await _dbContext.Accounts.AddAsync(account, cancellationToken);

        var accountCreatedEvent = new AccountCreatedEvent(
            account.Id,
            account.Email,
            account.FirstName,
            account.LastName,
            account.Role,
            account.IsActive,
            account.CreatedAt);

        await _dbContext.OutboxMessages.AddAsync(new OutboxMessage
        {
            AggregateType = nameof(Account),
            AggregateId = account.Id,
            EventType = typeof(AccountCreatedEvent).AssemblyQualifiedName!,
            Payload = System.Text.Json.JsonSerializer.Serialize(new
            {
                accountCreatedEvent.AccountId,
                accountCreatedEvent.Email,
                accountCreatedEvent.FirstName,
                accountCreatedEvent.LastName,
                accountCreatedEvent.Role,
                accountCreatedEvent.IsActive,
                accountCreatedEvent.CreatedAt,
            }),
            OccurredAt = accountCreatedEvent.OccurredAt,
        }, cancellationToken);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => _dbContext.Accounts.AnyAsync(x => x.Email == email, cancellationToken);
}
