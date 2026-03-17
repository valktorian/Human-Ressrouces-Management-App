using AspNetCoreRateLimit;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text.Json;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);

// ocelot.json
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);


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

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseIpRateLimiting();

app.Map("/swagger", swaggerApp =>
{
    swaggerApp.UseSwaggerUI(options =>
    {
        options.RoutePrefix = string.Empty;
        options.SwaggerEndpoint("/gateway-docs/v1/openapi.json", "WorkForceHub API");
    });
});

app.MapGet("/gateway-docs/v1/openapi.json", async (IHttpClientFactory httpClientFactory, CancellationToken ct) =>
{
    var client = httpClientFactory.CreateClient();
    var sources = new[]
    {
        ("Account Command", "http://host.docker.internal:5222/swagger/v1/swagger.json"),
        ("Account Query", "http://host.docker.internal:5115/swagger/v1/swagger.json"),
        ("Profile Command", "http://host.docker.internal:5001/swagger/v1/swagger.json"),
        ("Profile Query", "http://host.docker.internal:5004/swagger/v1/swagger.json")
    };

    var merged = new JsonObject
    {
        ["openapi"] = "3.0.1",
        ["info"] = new JsonObject
        {
            ["title"] = "WorkForceHub API",
            ["version"] = "v1",
            ["description"] = "Aggregated gateway documentation for AccountService and ProfileService."
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

    foreach (var (serviceName, url) in sources)
    {
        try
        {
            using var response = await client.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var json = JsonNode.Parse(await response.Content.ReadAsStringAsync(ct))?.AsObject();
            if (json is null)
            {
                continue;
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
      <p>Single gateway entry point for the aggregated API documentation and proxied service docs.</p>
      <ul>
        <li><a href="/swagger">Unified Swagger UI</a></li>
        <li><a href="/gateway-docs/v1/openapi.json">Unified OpenAPI JSON</a></li>
        <li><a href="/docs/account-command/index.html">Account Command Swagger</a></li>
        <li><a href="/docs/account-query/index.html">Account Query Swagger</a></li>
        <li><a href="/docs/profile-command/index.html">Profile Command Swagger</a></li>
        <li><a href="/docs/profile-query/index.html">Profile Query Swagger</a></li>
      </ul>
      <p>Gateway base URL: <code>http://localhost:5000</code></p>
    </body>
    </html>
    """,
    "text/html"));

app.MapWhen(
    context => !context.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase)
        && !context.Request.Path.StartsWithSegments("/gateway-docs", StringComparison.OrdinalIgnoreCase)
        && !context.Request.Path.StartsWithSegments("/docs", StringComparison.OrdinalIgnoreCase),
    branch => { branch.UseOcelot().GetAwaiter().GetResult(); });

app.Run();
