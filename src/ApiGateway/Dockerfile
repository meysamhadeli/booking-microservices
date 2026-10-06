# ---------- Build Stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution-level files
COPY .editorconfig .
COPY global.json .
COPY Directory.Build.props .
COPY Directory.Build.targets .

# Copy project files first (better Docker caching)
COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/ApiGateway/src/ApiGateway.csproj src/ApiGateway/src/
COPY src/Aspire/src/ServiceDefaults/ServiceDefaults.csproj src/Aspire/src/ServiceDefaults/

# Restore dependencies
RUN dotnet restore src/ApiGateway/src/ApiGateway.csproj

# Copy the rest of the source code
COPY src ./src

# Publish (build included)
RUN dotnet publish src/ApiGateway/src/ApiGateway.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---------- Runtime Stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=docker

EXPOSE 80

ENTRYPOINT ["dotnet", "ApiGateway.dll"]