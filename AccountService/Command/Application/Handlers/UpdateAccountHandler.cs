using AccountService.Command.Application.Abstractions;
using AccountService.Command.Application.Commands;
using AccountService.Command.Application.DTOs;
using Infrastructure.Api.Common;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Persistence;

namespace AccountService.Command.Application.Handlers;

public class UpdateAccountHandler : ICommandHandler<UpdateAccountCommand, AccountResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AccountResponse> HandleAsync(UpdateAccountCommand command, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(command.AccountId, cancellationToken)
            ?? throw ApiException.NotFound("Account not found.");

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        if (await _accountRepository.EmailExistsAsync(account.Id, normalizedEmail, cancellationToken))
        {
            throw new ApiException("An account with this email already exists.", 409);
        }

        account.UpdateProfile(normalizedEmail, command.FirstName, command.LastName);

        await _accountRepository.UpdateAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AccountResponse(
            account.Id,
            account.Email,
            account.FirstName,
            account.LastName,
            account.Role,
            account.IsActive,
            account.CreatedAt,
            account.UpdatedAt);
    }
}
