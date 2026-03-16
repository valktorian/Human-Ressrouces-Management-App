using AccountService.Command.Application.Abstractions;
using AccountService.Command.Application.Commands;
using AccountService.Command.Application.DTOs;
using Infrastructure.Api.Common;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Persistence;

namespace AccountService.Command.Application.Handlers;

public class UpdateAccountRoleHandler : ICommandHandler<UpdateAccountRoleCommand, AccountResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAccountRoleHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AccountResponse> HandleAsync(UpdateAccountRoleCommand command, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(command.AccountId, cancellationToken)
            ?? throw ApiException.NotFound("Account not found.");

        account.UpdateRole(command.Role);

        await _accountRepository.UpdateRoleAsync(account, cancellationToken);
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
