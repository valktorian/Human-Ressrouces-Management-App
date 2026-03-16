using AccountService.Command.Application.Abstractions;
using AccountService.Command.Domain;
using AccountService.Command.Domain.Events;
using AccountService.Command.Infrastructure.Persistence;
using Infrastructure.Api.Base;
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

        var accountCreatedEvent = new AccountCreatedEvent(account.Id, account.Email, account.FirstName, account.LastName, account.Role, account.IsActive, account.CreatedAt);
        await AddOutboxMessageAsync(account.Id, accountCreatedEvent, new
        {
            accountCreatedEvent.AccountId,
            accountCreatedEvent.Email,
            accountCreatedEvent.FirstName,
            accountCreatedEvent.LastName,
            accountCreatedEvent.Role,
            accountCreatedEvent.IsActive,
            accountCreatedEvent.CreatedAt,
        }, cancellationToken);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => _dbContext.Accounts.AnyAsync(x => x.Email == email, cancellationToken);

    public Task<bool> EmailExistsAsync(Guid excludedAccountId, string email, CancellationToken cancellationToken = default)
        => _dbContext.Accounts.AnyAsync(x => x.Id != excludedAccountId && x.Email == email, cancellationToken);

    public Task<Account?> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        => _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountId, cancellationToken);

    public Task<Account?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => _dbContext.Accounts.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        var evt = new AccountUpdatedEvent(account.Id, account.Email, account.FirstName, account.LastName, account.UpdatedAt);
        await AddOutboxMessageAsync(account.Id, evt, new
        {
            evt.AccountId,
            evt.Email,
            evt.FirstName,
            evt.LastName,
            evt.UpdatedAt,
        }, cancellationToken);
    }

    public async Task UpdateRoleAsync(Account account, CancellationToken cancellationToken = default)
    {
        var evt = new AccountRoleUpdatedEvent(account.Id, account.Role, account.UpdatedAt);
        await AddOutboxMessageAsync(account.Id, evt, new
        {
            evt.AccountId,
            evt.Role,
            evt.UpdatedAt,
        }, cancellationToken);
    }

    public async Task UpdatePasswordAsync(Account account, CancellationToken cancellationToken = default)
    {
        var evt = new AccountPasswordChangedEvent(account.Id, account.UpdatedAt);
        await AddOutboxMessageAsync(account.Id, evt, new
        {
            evt.AccountId,
            evt.UpdatedAt,
        }, cancellationToken);
    }

    public async Task DeleteAsync(Account account, CancellationToken cancellationToken = default)
    {
        _dbContext.Accounts.Remove(account);

        var evt = new AccountDeletedEvent(account.Id, DateTime.UtcNow);
        await AddOutboxMessageAsync(account.Id, evt, new
        {
            evt.AccountId,
            evt.DeletedAt,
        }, cancellationToken);
    }

    private Task AddOutboxMessageAsync(Guid aggregateId, BaseEvent evt, object payload, CancellationToken cancellationToken)
    {
        return _dbContext.OutboxMessages.AddAsync(new OutboxMessage
        {
            AggregateType = nameof(Account),
            AggregateId = aggregateId,
            EventType = evt.GetType().AssemblyQualifiedName!,
            Payload = System.Text.Json.JsonSerializer.Serialize(payload),
            OccurredAt = evt.OccurredAt,
        }, cancellationToken).AsTask();
    }
}
