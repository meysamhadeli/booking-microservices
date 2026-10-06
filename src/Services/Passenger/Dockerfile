# ---------- Build Stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution-level files
COPY .editorconfig .
COPY global.json .
COPY Directory.Build.props .
COPY Directory.Build.targets .

# Copy project files first (better Docker layer caching)
COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/Services/Passenger/src/Passenger/Passenger.csproj src/Services/Passenger/src/Passenger/
COPY src/Services/Passenger/src/Passenger.Api/Passenger.Api.csproj src/Services/Passenger/src/Passenger.Api/
COPY src/Aspire/src/ServiceDefaults/ServiceDefaults.csproj src/Aspire/src/ServiceDefaults/

# Restore dependencies
RUN dotnet restore src/Services/Passenger/src/Passenger.Api/Passenger.Api.csproj

# Copy remaining source code
COPY src ./src

# Publish (build included)
RUN dotnet publish src/Services/Passenger/src/Passenger.Api/Passenger.Api.csproj \
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

ENTRYPOINT ["dotnet", "Passenger.Api.dll"]