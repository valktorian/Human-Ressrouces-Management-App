namespace Infrastructure.Api.Authentication;

public interface ICurrentUserAccessor
{
    Guid GetRequiredAccountId();
}
