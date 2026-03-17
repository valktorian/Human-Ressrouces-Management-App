FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ProfileService/Query/Api/ProfileService.Query.Api.csproj", "ProfileService/Query/Api/"]
COPY ["Infrastructure.Api/Infrastructure.Api.csproj", "Infrastructure.Api/"]
COPY ["ProfileService/Query/Application/ProfileService.Query.Application.csproj", "ProfileService/Query/Application/"]
COPY ["ProfileService/Query/Domain/ProfileService.Query.Domain.csproj", "ProfileService/Query/Domain/"]
COPY ["ProfileService/Query/Infrastructure/ProfileService.Query.Infrastructure.csproj", "ProfileService/Query/Infrastructure/"]
RUN dotnet restore "ProfileService/Query/Api/ProfileService.Query.Api.csproj"
COPY . .
WORKDIR "/src/ProfileService/Query/Api"
RUN dotnet build "ProfileService.Query.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProfileService.Query.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProfileService.Query.Api.dll"]
