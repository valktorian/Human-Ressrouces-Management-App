FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["MediaService/Api/MediaService.Api.csproj", "MediaService/Api/"]
COPY ["Infrastructure.Api/Infrastructure.Api.csproj", "Infrastructure.Api/"]
RUN dotnet restore "MediaService/Api/MediaService.Api.csproj"
COPY . .
WORKDIR "/src/MediaService/Api"
RUN dotnet build "MediaService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MediaService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediaService.Api.dll"]
