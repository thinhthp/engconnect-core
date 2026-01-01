# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files (relative to solution root, but Docker build context will be repo root)
COPY EngConnect/EngConnect.Api/EngConnect.Api.csproj EngConnect/EngConnect.Api/
COPY EngConnect/EngConnect.Services/EngConnect.Services.csproj EngConnect/EngConnect.Services/
COPY EngConnect/EngConnect.Repositories/EngConnect.Repositories.csproj EngConnect/EngConnect.Repositories/
COPY EngConnect/EngConnect.Entities/EngConnect.Entities.csproj EngConnect/EngConnect.Entities/

# Restore dependencies
RUN dotnet restore EngConnect/EngConnect.Api/EngConnect.Api.csproj

# Copy the rest of the source
COPY . .

# Publish
WORKDIR /src/EngConnect/EngConnect.Api
RUN dotnet publish EngConnect.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Azure App Service sets PORT, but this is fine
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "EngConnect.Api.dll"]