FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["WorkForceHub.Gateway/WorkForceHub.Gateway.csproj", "WorkForceHub.Gateway/"]
RUN dotnet restore "WorkForceHub.Gateway/WorkForceHub.Gateway.csproj"
COPY . .
WORKDIR "/src/WorkForceHub.Gateway"
RUN dotnet build "WorkForceHub.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkForceHub.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkForceHub.Gateway.dll"]
