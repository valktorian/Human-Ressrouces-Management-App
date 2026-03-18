using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Api.Extensions;

public static class WebApplicationMigrationExtensions
{
    public static async Task<WebApplication> ApplyMigrationsAsync<TDbContext>(
        this WebApplication app,
        CancellationToken cancellationToken = default)
        where TDbContext : DbContext
    {
        await using var scope = app.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(typeof(TDbContext));

        logger.LogInformation("Ensuring EF Core migrations are applied for {DbContext}.", typeof(TDbContext).Name);

        await dbContext.Database.MigrateAsync(cancellationToken);

        logger.LogInformation("EF Core migrations are up to date for {DbContext}.", typeof(TDbContext).Name);

        return app;
    }
}
