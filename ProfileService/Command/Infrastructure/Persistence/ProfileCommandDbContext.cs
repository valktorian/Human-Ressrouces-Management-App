using Infrastructure.Api.Messaging;
using Microsoft.EntityFrameworkCore;
using ProfileService.Command.Domain;

namespace ProfileService.Command.Infrastructure.Persistence;

public class ProfileCommandDbContext : DbContext
{
    public const string Schema = "profile_command";

    public ProfileCommandDbContext(DbContextOptions<ProfileCommandDbContext> options)
        : base(options)
    {
    }

    public DbSet<Profile> Profiles => Set<Profile>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProfileCommandDbContext).Assembly);
    }
}
