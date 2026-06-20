using AspNetCoreRateLimit;
using Microsoft.Extensions.Caching.Memory;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Polly;
using Polly.Extensions.Http;
using System.Text.Json;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);

var gatewayMode = builder.Configuration["GatewayMode"] ?? builder.Environment.EnvironmentName;
var ocelotFileName = gatewayMode.Equals("Docker", StringComparison.OrdinalIgnoreCase) ? "ocelot.docker.json" : "ocelot.json";
builder.Configuration.AddJsonFile(ocelotFileName, optional: false, reloadOnChange: true);

var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

builder.Services.AddHttpClient("SwaggerClient").AddPolicyHandler(retryPolicy);
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

var swaggerSources = builder.Configuration.GetSection("SwaggerSources").Get<Dictionary<string, string[]>>() ?? new Dictionary<string, string[]>();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseIpRateLimiting();

app.Map("/swagger", swaggerApp =>
{
    swaggerApp.UseSwaggerUI(options =>
    {
        options.RoutePrefix = string.Empty;
        options.SwaggerEndpoint("/gateway-docs/v1/openapi.json", "WorkForceHub Unified");
    });
});

app.MapGet("/gateway-docs/v1/openapi.json", async (IHttpClientFactory httpClientFactory, IMemoryCache cache, CancellationToken ct) =>
{
    return await cache.GetOrCreateAsync("unified_swagger", async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
        var client = httpClientFactory.CreateClient("SwaggerClient");
        var merged = new JsonObject { ["openapi"] = "3.0.1", ["paths"] = new JsonObject() };
        
        foreach (var (serviceName, urls) in swaggerSources)
        {
            var json = await TryFetchSwaggerDocumentAsync(client, urls, ct);
            if (json?["paths"] is JsonObject paths) 
                foreach (var path in paths) merged["paths"]![path.Key] = path.Value?.DeepClone();
        }
        return merged;
    });
});

app.MapWhen(ctx => !ctx.Request.Path.StartsWithSegments("/swagger") && !ctx.Request.Path.StartsWithSegments("/gateway-docs"), branch => branch.UseOcelot());

app.Run();

static async Task<JsonObject?> TryFetchSwaggerDocumentAsync(HttpClient client, IEnumerable<string> urls, CancellationToken ct)
{
    foreach (var url in urls)
    {
        try { var response = await client.GetAsync(url, ct); if (response.IsSuccessStatusCode) return JsonNode.Parse(await response.Content.ReadAsStringAsync(ct))?.AsObject(); } catch { }
    }
    return null;
}