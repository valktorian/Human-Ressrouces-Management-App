using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Api.Authentication;

public class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetRequiredAccountId()
    {
        var principal = _httpContextAccessor.HttpContext?.User;
        var identifier = principal?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal?.FindFirstValue("sub");

        if (!Guid.TryParse(identifier, out var accountId))
        {
            throw new InvalidOperationException("Authenticated account identifier is missing.");
        }

        return accountId;
    }
}
