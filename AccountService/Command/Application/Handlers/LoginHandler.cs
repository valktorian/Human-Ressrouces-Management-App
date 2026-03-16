using AccountService.Command.Application.Abstractions;
using AccountService.Command.Application.Commands;
using AccountService.Command.Application.DTOs;
using AccountService.Command.Domain;
using Infrastructure.Api.Common;
using Infrastructure.Api.Messaging;
using Microsoft.AspNetCore.Identity;

namespace AccountService.Command.Application.Handlers;

public class LoginHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher<Account> _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginHandler(
        IAccountRepository accountRepository,
        IPasswordHasher<Account> passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _accountRepository = accountRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var account = await _accountRepository.GetByEmailAsync(normalizedEmail, cancellationToken)
            ?? throw ApiException.Unauthorized("Invalid email or password.");

        var verificationResult = _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, command.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw ApiException.Unauthorized("Invalid email or password.");
        }

        var token = _jwtTokenGenerator.GenerateToken(account);

        return new LoginResponse(
            token,
            _jwtTokenGenerator.GetExpirationUtc(),
            account.Id,
            account.Email,
            account.Role);
    }
}
