FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ProfileService/Command/Api/ProfileService.Command.Api.csproj", "ProfileService/Command/Api/"]
COPY ["Infrastructure.Api/Infrastructure.Api.csproj", "Infrastructure.Api/"]
COPY ["ProfileService/Command/Application/ProfileService.Command.Application.csproj", "ProfileService/Command/Application/"]
COPY ["ProfileService/Command/Domain/ProfileService.Command.Domain.csproj", "ProfileService/Command/Domain/"]
COPY ["ProfileService/Command/Infrastructure/ProfileService.Command.Infrastructure.csproj", "ProfileService/Command/Infrastructure/"]
RUN dotnet restore "ProfileService/Command/Api/ProfileService.Command.Api.csproj"
COPY . .
WORKDIR "/src/ProfileService/Command/Api"
RUN dotnet build "ProfileService.Command.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProfileService.Command.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProfileService.Command.Api.dll"]
