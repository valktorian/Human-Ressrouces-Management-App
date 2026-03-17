FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["AccountService/Query/Api/AccountService.Query.Api.csproj", "AccountService/Query/Api/"]
COPY ["Infrastructure.Api/Infrastructure.Api.csproj", "Infrastructure.Api/"]
COPY ["AccountService/Query/Application/AccountService.Query.Application.csproj", "AccountService/Query/Application/"]
COPY ["AccountService/Query/Domain/AccountService.Query.Domain.csproj", "AccountService/Query/Domain/"]
COPY ["AccountService/Query/Infrastructure/AccountService.Query.Infrastructure.csproj", "AccountService/Query/Infrastructure/"]
RUN dotnet restore "AccountService/Query/Api/AccountService.Query.Api.csproj"
COPY . .
WORKDIR "/src/AccountService/Query/Api"
RUN dotnet build "AccountService.Query.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AccountService.Query.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AccountService.Query.Api.dll"]
