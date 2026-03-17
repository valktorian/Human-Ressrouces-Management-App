FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["EvolutionService/Query/Api/EvolutionService.Query.Api.csproj", "EvolutionService/Query/Api/"]
COPY ["Infrastructure.Api/Infrastructure.Api.csproj", "Infrastructure.Api/"]
COPY ["EvolutionService/Query/Application/EvolutionService.Query.Application.csproj", "EvolutionService/Query/Application/"]
COPY ["EvolutionService/Query/Domain/EvolutionService.Query.Domain.csproj", "EvolutionService/Query/Domain/"]
COPY ["EvolutionService/Query/Infrastructure/EvolutionService.Query.Infrastructure.csproj", "EvolutionService/Query/Infrastructure/"]
RUN dotnet restore "EvolutionService/Query/Api/EvolutionService.Query.Api.csproj"
COPY . .
WORKDIR "/src/EvolutionService/Query/Api"
RUN dotnet build "EvolutionService.Query.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EvolutionService.Query.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EvolutionService.Query.Api.dll"]
