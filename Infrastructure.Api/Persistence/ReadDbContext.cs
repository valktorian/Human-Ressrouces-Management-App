using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Api.Persistence;

public class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options)
        : base(options) { }
}
