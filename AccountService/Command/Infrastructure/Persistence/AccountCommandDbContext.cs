using AccountService.Command.Domain;
using Infrastructure.Api.Messaging;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Command.Infrastructure.Persistence;

public class AccountCommandDbContext : DbContext
{
    public const string Schema = "account_command";

    public AccountCommandDbContext(DbContextOptions<AccountCommandDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountCommandDbContext).Assembly);
    }
}
