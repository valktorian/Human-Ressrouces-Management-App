using AccountService.Command.Application.Abstractions;
using AccountService.Command.Application.Commands;
using AccountService.Command.Domain;
using Infrastructure.Api.Common;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Persistence;
using Microsoft.AspNetCore.Identity;

namespace AccountService.Command.Application.Handlers;

public class ChangeAccountPasswordHandler : ICommandHandler<ChangeAccountPasswordCommand, bool>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<Account> _passwordHasher;

    public ChangeAccountPasswordHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher<Account> passwordHasher)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> HandleAsync(ChangeAccountPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(command.AccountId, cancellationToken)
            ?? throw ApiException.NotFound("Account not found.");

        account.SetPasswordHash(_passwordHasher.HashPassword(account, command.Password));

        await _accountRepository.UpdatePasswordAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
