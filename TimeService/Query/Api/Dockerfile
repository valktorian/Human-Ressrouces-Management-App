FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TimeService/Query/Api/TimeService.Query.Api.csproj", "TimeService/Query/Api/"]
COPY ["Infrastructure.Api/Infrastructure.Api.csproj", "Infrastructure.Api/"]
COPY ["TimeService/Query/Application/TimeService.Query.Application.csproj", "TimeService/Query/Application/"]
COPY ["TimeService/Query/Domain/TimeService.Query.Domain.csproj", "TimeService/Query/Domain/"]
COPY ["TimeService/Query/Infrastructure/TimeService.Query.Infrastructure.csproj", "TimeService/Query/Infrastructure/"]
RUN dotnet restore "TimeService/Query/Api/TimeService.Query.Api.csproj"
COPY . .
WORKDIR "/src/TimeService/Query/Api"
RUN dotnet build "TimeService.Query.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TimeService.Query.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimeService.Query.Api.dll"]
