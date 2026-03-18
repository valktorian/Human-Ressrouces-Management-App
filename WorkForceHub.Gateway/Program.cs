using AspNetCoreRateLimit;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text.Json;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);

var gatewayMode = builder.Configuration["GatewayMode"] ?? builder.Environment.EnvironmentName;
var ocelotFileName = gatewayMode.Equals("Docker", StringComparison.OrdinalIgnoreCase)
    ? "ocelot.docker.json"
    : "ocelot.json";

builder.Configuration.AddJsonFile(ocelotFileName, optional: false, reloadOnChange: true);


builder.Services.AddHttpClient();



builder.Services.AddEndpointsApiExplorer();
// Services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// rate limit services (AspNetCoreRateLimit)
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

var swaggerSources = builder.Configuration
    .GetSection("SwaggerSources")
    .Get<Dictionary<string, string[]>>() ?? new Dictionary<string, string[]>();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseIpRateLimiting();

app.Map("/swagger", swaggerApp =>
{
    swaggerApp.UseSwaggerUI(options =>
    {
        options.RoutePrefix = string.Empty;
        options.DocumentTitle = "WorkForceHub API Docs";
        options.SwaggerEndpoint("/gateway-docs/v1/openapi.json", "WorkForceHub Unified");
        options.SwaggerEndpoint("/gateway-docs/account-command/openapi.json", "Account Command");
        options.SwaggerEndpoint("/gateway-docs/account-query/openapi.json", "Account Query");
        options.SwaggerEndpoint("/gateway-docs/profile-command/openapi.json", "Profile Command");
        options.SwaggerEndpoint("/gateway-docs/profile-query/openapi.json", "Profile Query");
        options.SwaggerEndpoint("/gateway-docs/time-command/openapi.json", "Time Command");
        options.SwaggerEndpoint("/gateway-docs/time-query/openapi.json", "Time Query");
    });
});

app.MapGet("/gateway-docs/v1/openapi.json", async (IHttpClientFactory httpClientFactory, CancellationToken ct) =>
{
    var client = httpClientFactory.CreateClient();
    var sources = new[]
    {
        ("Account Command", swaggerSources["account-command"]),
        ("Account Query", swaggerSources["account-query"]),
        ("Profile Command", swaggerSources["profile-command"]),
        ("Profile Query", swaggerSources["profile-query"]),
        ("Time Command", swaggerSources["time-command"]),
        ("Time Query", swaggerSources["time-query"])
    };

    var merged = new JsonObject
    {
        ["openapi"] = "3.0.1",
        ["info"] = new JsonObject
        {
            ["title"] = "WorkForceHub API",
            ["version"] = "v1",
            ["description"] = "Aggregated gateway documentation for all WorkForceHub services."
        },
        ["paths"] = new JsonObject(),
        ["components"] = new JsonObject
        {
            ["schemas"] = new JsonObject(),
            ["securitySchemes"] = new JsonObject()
        },
        ["tags"] = new JsonArray()
    };

    var mergedPaths = merged["paths"]!.AsObject();
    var mergedComponents = merged["components"]!.AsObject();
    var mergedSchemas = mergedComponents["schemas"]!.AsObject();
    var mergedSecuritySchemes = mergedComponents["securitySchemes"]!.AsObject();
    var mergedTags = merged["tags"]!.AsArray();
    var seenTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    foreach (var (serviceName, urls) in sources)
    {
        try
        {
            var json = await TryFetchSwaggerDocumentAsync(client, urls, ct);
            if (json is null)
            {
                throw new InvalidOperationException($"Unable to fetch Swagger document for {serviceName}.");
            }

            if (json["paths"] is JsonObject paths)
            {
                foreach (var path in paths)
                {
                    if (path.Value is not JsonObject pathObject)
                    {
                        continue;
                    }

                    if (!mergedPaths.TryGetPropertyValue(path.Key, out var existingNode) || existingNode is not JsonObject existingPath)
                    {
                        mergedPaths[path.Key] = pathObject.DeepClone();
                        continue;
                    }

                    foreach (var operation in pathObject)
                    {
                        existingPath[operation.Key] = operation.Value?.DeepClone();
                    }
                }
            }

            if (json["components"] is JsonObject components)
            {
                if (components["schemas"] is JsonObject schemas)
                {
                    foreach (var schema in schemas)
                    {
                        mergedSchemas[schema.Key] = schema.Value?.DeepClone();
                    }
                }

                if (components["securitySchemes"] is JsonObject securitySchemes)
                {
                    foreach (var scheme in securitySchemes)
                    {
                        mergedSecuritySchemes[scheme.Key] = scheme.Value?.DeepClone();
                    }
                }
            }

            if (json["tags"] is JsonArray tags)
            {
                foreach (var tagNode in tags)
                {
                    if (tagNode is not JsonObject tagObject)
                    {
                        continue;
                    }

                    var tagName = tagObject["name"]?.GetValue<string>();
                    if (string.IsNullOrWhiteSpace(tagName) || !seenTags.Add(tagName))
                    {
                        continue;
                    }

                    mergedTags.Add(tagObject.DeepClone());
                }
            }
        }
        catch (Exception ex)
        {
            mergedTags.Add(new JsonObject
            {
                ["name"] = $"{serviceName} unavailable",
                ["description"] = ex.Message
            });
        }
    }

    return Results.Json(merged, new JsonSerializerOptions(JsonSerializerDefaults.Web));
});

app.MapGet("/gateway-docs/{service}/openapi.json", async (string service, IHttpClientFactory httpClientFactory, CancellationToken ct) =>
{
    if (!swaggerSources.TryGetValue(service, out var urls))
    {
        return Results.NotFound(new { message = $"Unknown Swagger source '{service}'." });
    }

    var client = httpClientFactory.CreateClient();
    var json = await TryFetchSwaggerDocumentAsync(client, urls, ct);

    return json is null
        ? Results.Problem($"Unable to fetch Swagger document for '{service}'.")
        : Results.Json(json, new JsonSerializerOptions(JsonSerializerDefaults.Web));
});

app.MapGet("/docs", () => Results.Content(
    """
    <!doctype html>
    <html lang="en">
    <head>
      <meta charset="utf-8" />
      <title>WorkForceHub Docs</title>
      <style>
        body { font-family: Segoe UI, sans-serif; margin: 2rem; color: #1f2937; }
        h1 { margin-bottom: 0.5rem; }
        ul { padding-left: 1.25rem; }
        a { color: #0f766e; text-decoration: none; }
        a:hover { text-decoration: underline; }
        code { background: #f3f4f6; padding: 0.15rem 0.35rem; border-radius: 4px; }
      </style>
    </head>
    <body>
      <h1>WorkForceHub API Docs</h1>
      <p>Single gateway entry point for all service documentation.</p>
      <ul>
        <li><a href="/swagger">Swagger UI (all services in one page)</a></li>
        <li><a href="/gateway-docs/v1/openapi.json">Unified OpenAPI JSON</a></li>
        <li><a href="/gateway-docs/account-command/openapi.json">Account Command OpenAPI</a></li>
        <li><a href="/gateway-docs/account-query/openapi.json">Account Query OpenAPI</a></li>
        <li><a href="/gateway-docs/profile-command/openapi.json">Profile Command OpenAPI</a></li>
        <li><a href="/gateway-docs/profile-query/openapi.json">Profile Query OpenAPI</a></li>
        <li><a href="/gateway-docs/time-command/openapi.json">Time Command OpenAPI</a></li>
        <li><a href="/gateway-docs/time-query/openapi.json">Time Query OpenAPI</a></li>
      </ul>
      <p>Gateway base URL: <code>http://localhost:5000</code></p>
    </body>
    </html>
    """,
    "text/html"));

app.MapWhen(
    context => !context.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase)
        && !context.Request.Path.StartsWithSegments("/gateway-docs", StringComparison.OrdinalIgnoreCase)
        && !context.Request.Path.Equals("/docs", StringComparison.OrdinalIgnoreCase)
        && !context.Request.Path.Equals("/docs/", StringComparison.OrdinalIgnoreCase),
    branch => { branch.UseOcelot().GetAwaiter().GetResult(); });

app.Run();

static async Task<JsonObject?> TryFetchSwaggerDocumentAsync(HttpClient client, IEnumerable<string> urls, CancellationToken ct)
{
    foreach (var url in urls)
    {
        try
        {
            using var response = await client.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                continue;
            }

            var document = JsonNode.Parse(await response.Content.ReadAsStringAsync(ct))?.AsObject();
            if (document is not null)
            {
                return document;
            }
        }
        catch
        {
        }
    }

    return null;
}
