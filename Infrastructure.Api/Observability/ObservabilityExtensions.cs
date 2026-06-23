using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Infrastructure.Api.Observability;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddWorkForceHubTracing(this IServiceCollection services, IConfiguration configuration, string serviceName)
    {
        var endpoint = configuration["Jaeger:OtlpEndpoint"] ?? "http://localhost:4317";

        services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(opts => opts.Endpoint = new Uri(endpoint)));

        return services;
    }
}
