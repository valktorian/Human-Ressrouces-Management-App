using AccountService.Command.Domain;

namespace AccountService.Command.Application.Abstractions;

public interface IAccountRepository
{
    Task AddAsync(Account account, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
