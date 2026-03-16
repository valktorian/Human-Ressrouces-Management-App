using AccountService.Command.Domain;

namespace AccountService.Command.Application.Abstractions;

public interface IAccountRepository
{
    Task AddAsync(Account account, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(Guid excludedAccountId, string email, CancellationToken cancellationToken = default);

    Task<Account?> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default);

    Task<Account?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task UpdateAsync(Account account, CancellationToken cancellationToken = default);

    Task UpdateRoleAsync(Account account, CancellationToken cancellationToken = default);

    Task UpdatePasswordAsync(Account account, CancellationToken cancellationToken = default);

    Task DeleteAsync(Account account, CancellationToken cancellationToken = default);
}
