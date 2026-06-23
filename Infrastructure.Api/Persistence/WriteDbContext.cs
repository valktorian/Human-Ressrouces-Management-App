using Infrastructure.Api.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Api.Persistence;

public class WriteDbContext : DbContext
{
    public WriteDbContext(DbContextOptions<WriteDbContext> options)
        : base(options) { }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
}
