FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["HrmApp.Gateway/HrmApp.Gateway.csproj", "HrmApp.Gateway/"]
RUN dotnet restore "HrmApp.Gateway/HrmApp.Gateway.csproj"
COPY . .
WORKDIR "/src/HrmApp.Gateway"
RUN dotnet build "HrmApp.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HrmApp.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkForceHub.Gateway.dll"]
