namespace Infrastructure.Api.Authentication;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "WorkForceHub";

    public string Audience { get; set; } = "WorkForceHub.Client";

    public string SecretKey { get; set; } = string.Empty;

    public int ExpirationMinutes { get; set; } = 60;
}
