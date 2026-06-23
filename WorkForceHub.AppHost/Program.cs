var builder = DistributedApplication.CreateBuilder(args);

const string localJwtSecret = "WorkForceHub-Local-Development-Secret-Key-2026";
const string localInternalApiKey = "WorkForceHub-Local-Internal-Media-Key-2026";

AddService("gateway", @"..\WorkForceHub.Gateway\WorkForceHub.Gateway.csproj")
    .WithEnvironment("GatewayMode", "Local");

AddService("account-command", @"..\AccountService\Command\Api\AccountService.Command.Api.csproj");
AddService("account-query", @"..\AccountService\Query\Api\AccountService.Query.Api.csproj");
AddService("profile-command", @"..\ProfileService\Command\Api\ProfileService.Command.Api.csproj");
AddService("profile-query", @"..\ProfileService\Query\Api\ProfileService.Query.Api.csproj");
AddService("time-command", @"..\TimeService\Command\Api\TimeService.Command.Api.csproj");
AddService("time-query", @"..\TimeService\Query\Api\TimeService.Query.Api.csproj");
AddService("evolution-command", @"..\EvolutionService\Command\Api\EvolutionService.Command.Api.csproj");
AddService("evolution-query", @"..\EvolutionService\Query\Api\EvolutionService.Query.Api.csproj");
AddService("media", @"..\MediaService\Api\MediaService.Api.csproj");

builder.Build().Run();

IResourceBuilder<ProjectResource> AddService(string name, string projectPath)
{
    return builder.AddProject(name, projectPath, launchProfileName: "http")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
        .WithEnvironment("DOTNET_ENVIRONMENT", "Development")
        .WithEnvironment("Jwt__SecretKey", localJwtSecret)
        .WithEnvironment("Storage__InternalApiKey", localInternalApiKey);
}
