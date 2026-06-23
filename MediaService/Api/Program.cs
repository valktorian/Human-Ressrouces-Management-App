using Infrastructure.Api.Authentication;
using Infrastructure.Api.HealthChecks;
using Infrastructure.Api.Middleware;
using Infrastructure.Api.Observability;
using MediaService.Api.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddWorkForceHubTracing(builder.Configuration, "MediaService");
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddWorkForceHubSwagger("WorkForceHub Media API");
builder.Services.Configure<LocalMediaStorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.AddSingleton<ILocalMediaStorage, LocalMediaStorage>();

var app = builder.Build();

var storage = app.Services.GetRequiredService<ILocalMediaStorage>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalErrorHandler();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(storage.RootPath),
    RequestPath = "/media"
});
app.MapHealthChecks("/health", HealthCheckExtensions.DefaultOptions);
app.MapControllers();

app.Run();
