var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => Results.Ok(new
{
    Service = "ProfileService.Command",
    Status = "running",
}));

app.MapGet("/health", () => Results.Ok(new
{
    Service = "ProfileService.Command",
    Status = "healthy",
}));

app.Run();
