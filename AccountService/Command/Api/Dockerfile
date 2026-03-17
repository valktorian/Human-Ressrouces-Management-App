FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["AccountService/Command/Api/AccountService.Command.Api.csproj", "AccountService/Command/Api/"]
COPY ["Infrastructure.Api/Infrastructure.Api.csproj", "Infrastructure.Api/"]
COPY ["AccountService/Command/Application/AccountService.Command.Application.csproj", "AccountService/Command/Application/"]
COPY ["AccountService/Command/Domain/AccountService.Command.Domain.csproj", "AccountService/Command/Domain/"]
COPY ["AccountService/Command/Infrastructure/AccountService.Command.Infrastructure.csproj", "AccountService/Command/Infrastructure/"]
RUN dotnet restore "AccountService/Command/Api/AccountService.Command.Api.csproj"
COPY . .
WORKDIR "/src/AccountService/Command/Api"
RUN dotnet build "AccountService.Command.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AccountService.Command.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AccountService.Command.Api.dll"]
