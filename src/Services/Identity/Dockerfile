# ---------- Build Stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution-level files
COPY .editorconfig .
COPY global.json .
COPY Directory.Build.props .
COPY Directory.Build.targets .

# Copy project files first (for better Docker layer caching)
COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/Services/Identity/src/Identity/Identity.csproj src/Services/Identity/src/Identity/
COPY src/Services/Identity/src/Identity.Api/Identity.Api.csproj src/Services/Identity/src/Identity.Api/
COPY src/Aspire/src/ServiceDefaults/ServiceDefaults.csproj src/Aspire/src/ServiceDefaults/

# Restore dependencies
RUN dotnet restore src/Services/Identity/src/Identity.Api/Identity.Api.csproj

# Copy remaining source code
COPY src ./src

# Publish (build included, no separate build step needed)
RUN dotnet publish src/Services/Identity/src/Identity.Api/Identity.Api.csproj \
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

ENTRYPOINT ["dotnet", "Identity.Api.dll"]