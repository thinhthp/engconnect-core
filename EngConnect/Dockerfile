# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first (leverages layer caching)
COPY EngConnect.Api/EngConnect.Api.csproj EngConnect.Api/
COPY EngConnect.Services/EngConnect.Services.csproj EngConnect.Services/
COPY EngConnect.Repositories/EngConnect.Repositories.csproj EngConnect.Repositories/
COPY EngConnect.Entities/EngConnect.Entities.csproj EngConnect.Entities/

# Restore dependencies
RUN dotnet restore EngConnect.Api/EngConnect.Api.csproj

# Copy the rest of the source
COPY . .

# Publish (no app host = portable, smaller)
WORKDIR /src/EngConnect.Api
RUN dotnet publish EngConnect.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Optional: set environment (Render sets ASPNETCORE_ENVIRONMENT separately if you configure it)
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
# If you want to ensure Production:
# ENV ASPNETCORE_ENVIRONMENT=Production

# Copy published output
COPY --from=build /app/publish .

# Expose the port Render will probe (Render usually sets PORT=10000 internally, but 8080 is fine; it auto-detects)
EXPOSE 8080

# (Optional) Healthcheck (adjust path if you add a real one)
# HEALTHCHECK --interval=30s --timeout=3s CMD wget -qO- http://127.0.0.1:8080/health || exit 1

ENTRYPOINT ["dotnet", "EngConnect.Api.dll"]