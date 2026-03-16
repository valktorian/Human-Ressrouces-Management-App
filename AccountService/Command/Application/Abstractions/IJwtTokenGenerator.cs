using AccountService.Command.Domain;

namespace AccountService.Command.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateToken(Account account);

    DateTime GetExpirationUtc();
}
