var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => Results.Ok(new
{
    Service = "EvolutionService.Query",
    Status = "running",
}));

app.MapGet("/health", () => Results.Ok(new
{
    Service = "EvolutionService.Query",
    Status = "healthy",
}));

app.Run();
