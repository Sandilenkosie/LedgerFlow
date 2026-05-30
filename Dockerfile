# Multi-stage Dockerfile for LedgerFlow (build and runtime)
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["LedgerFlow.slnx", "./"]
COPY ["WebUI/WebUI.csproj", "WebUI/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restore packages
RUN dotnet restore "LedgerFlow.slnx"

# Copy the remaining files and publish
COPY . .
WORKDIR /src/WebUI
RUN dotnet publish "WebUI.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "WebUI.dll"]
