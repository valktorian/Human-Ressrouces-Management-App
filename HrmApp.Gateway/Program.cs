using AspNetCoreRateLimit;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ocelot.json
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);


builder.Services.AddOpenApi();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseIpRateLimiting();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "WorkForceHub Gateway");
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
      <p>Single gateway entry point for the current account service documentation and routes.</p>
      <ul>
        <li><a href="/swagger">Gateway Swagger</a></li>
        <li><a href="/docs/account-command/index.html">Account Command Swagger</a></li>
        <li><a href="/docs/account-query/index.html">Account Query Swagger</a></li>
      </ul>
      <p>Gateway base URL: <code>http://localhost:5000</code></p>
    </body>
    </html>
    """,
    "text/html"));

await app.UseOcelot();
app.Run();
