using Infrastructure.Api.Base;

namespace AccountService.Command.Domain;

public class Account : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Account()
    {
    }

    public static Account Create(string email, string firstName, string lastName, string role)
    {
        var now = DateTime.UtcNow;

        return new Account
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Role = role.Trim(),
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }
}
