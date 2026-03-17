using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ProfileService.Command.Infrastructure.Persistence;

public class ProfileCommandDbContextFactory : IDesignTimeDbContextFactory<ProfileCommandDbContext>
{
    public ProfileCommandDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Api"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing.");

        var optionsBuilder = new DbContextOptionsBuilder<ProfileCommandDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new ProfileCommandDbContext(optionsBuilder.Options);
    }
}
