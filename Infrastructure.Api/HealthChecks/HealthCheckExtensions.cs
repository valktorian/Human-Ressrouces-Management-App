using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure.Api.HealthChecks;

public static class HealthCheckExtensions
{
    public static HealthCheckOptions DefaultOptions => new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    duration = Math.Round(e.Value.Duration.TotalMilliseconds, 2)
                })
            });
        }
    };
}
