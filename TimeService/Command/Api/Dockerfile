FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TimeService/Command/Api/TimeService.Command.Api.csproj", "TimeService/Command/Api/"]
COPY ["Infrastructure.Api/Infrastructure.Api.csproj", "Infrastructure.Api/"]
COPY ["TimeService/Command/Application/TimeService.Command.Application.csproj", "TimeService/Command/Application/"]
COPY ["TimeService/Command/Domain/TimeService.Command.Domain.csproj", "TimeService/Command/Domain/"]
COPY ["TimeService/Command/Infrastructure/TimeService.Command.Infrastructure.csproj", "TimeService/Command/Infrastructure/"]
RUN dotnet restore "TimeService/Command/Api/TimeService.Command.Api.csproj"
COPY . .
WORKDIR "/src/TimeService/Command/Api"
RUN dotnet build "TimeService.Command.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TimeService.Command.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimeService.Command.Api.dll"]
