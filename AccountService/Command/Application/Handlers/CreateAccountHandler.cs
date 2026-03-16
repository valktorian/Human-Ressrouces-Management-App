using AccountService.Command.Application.Abstractions;
using AccountService.Command.Application.Commands;
using AccountService.Command.Application.DTOs;
using AccountService.Command.Domain;
using Infrastructure.Api.Common;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Persistence;
using Microsoft.AspNetCore.Identity;

namespace AccountService.Command.Application.Handlers;

public class CreateAccountHandler : ICommandHandler<CreateAccountCommand, CreateAccountResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<Account> _passwordHasher;

    public CreateAccountHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher<Account> passwordHasher)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<CreateAccountResponse> HandleAsync(
        CreateAccountCommand request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await _accountRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new ApiException("An account with this email already exists.", 409);
        }

        var account = Account.Create(
            normalizedEmail,
            request.FirstName,
            request.LastName,
            request.Role);

        account.SetPasswordHash(_passwordHasher.HashPassword(account, request.Password));

        await _accountRepository.AddAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateAccountResponse(
            account.Id,
            account.Email,
            account.FirstName,
            account.LastName,
            account.Role,
            account.IsActive,
            account.CreatedAt);
    }
}
