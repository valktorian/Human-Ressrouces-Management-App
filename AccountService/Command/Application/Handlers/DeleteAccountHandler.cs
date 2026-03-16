using AccountService.Command.Application.Abstractions;
using AccountService.Command.Application.Commands;
using Infrastructure.Api.Common;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Persistence;

namespace AccountService.Command.Application.Handlers;

public class DeleteAccountHandler : ICommandHandler<DeleteAccountCommand, bool>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(DeleteAccountCommand command, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(command.AccountId, cancellationToken)
            ?? throw ApiException.NotFound("Account not found.");

        await _accountRepository.DeleteAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
