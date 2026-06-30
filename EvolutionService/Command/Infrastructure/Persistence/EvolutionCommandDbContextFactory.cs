using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EvolutionService.Command.Infrastructure.Persistence;

public class EvolutionCommandDbContextFactory : IDesignTimeDbContextFactory<EvolutionCommandDbContext>
{
    public EvolutionCommandDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is required.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<EvolutionCommandDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new EvolutionCommandDbContext(optionsBuilder.Options);
    }
}
