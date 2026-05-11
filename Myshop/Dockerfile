# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy project files
COPY ["Myshop.Api/Myshop.Api.csproj", "Myshop.Api/"]
COPY ["Myshop.Core/Myshop.Core.csproj", "Myshop.Core/"]
COPY ["Myshop.Application/Myshop.Application.csproj", "Myshop.Application/"]
COPY ["Myshop.Infrastructure/Myshop.Infrastructure.csproj", "Myshop.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Myshop.Api/Myshop.Api.csproj"

# Copy all source code
COPY . .

# Build
WORKDIR "/src/Myshop.Api"
RUN dotnet build "Myshop.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Myshop.Api.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=5s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/api/health || exit 1

# Environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "Myshop.Api.dll"]
