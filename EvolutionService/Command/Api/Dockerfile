FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["EvolutionService/Command/Api/EvolutionService.Command.Api.csproj", "EvolutionService/Command/Api/"]
COPY ["Infrastructure.Api/Infrastructure.Api.csproj", "Infrastructure.Api/"]
COPY ["EvolutionService/Command/Application/EvolutionService.Command.Application.csproj", "EvolutionService/Command/Application/"]
COPY ["EvolutionService/Command/Domain/EvolutionService.Command.Domain.csproj", "EvolutionService/Command/Domain/"]
COPY ["EvolutionService/Command/Infrastructure/EvolutionService.Command.Infrastructure.csproj", "EvolutionService/Command/Infrastructure/"]
RUN dotnet restore "EvolutionService/Command/Api/EvolutionService.Command.Api.csproj"
COPY . .
WORKDIR "/src/EvolutionService/Command/Api"
RUN dotnet build "EvolutionService.Command.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EvolutionService.Command.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EvolutionService.Command.Api.dll"]
