FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder
WORKDIR /app

# Setup working directory for the project
WORKDIR /app
COPY ./src/BuildingBlocks/BuildingBlocks.csproj ./BuildingBlocks/
COPY ./src/Services/Flight/src/Flight/Flight.csproj ./Services/Flight/src/Flight/
COPY ./src/Services/Flight/src/Flight.Api/Flight.Api.csproj ./Services/Flight/src/Flight.Api/


# Restore nuget packages
RUN dotnet restore ./Services/Flight/src/Flight.Api/Flight.Api.csproj

# Copy project files
COPY ./src/BuildingBlocks ./BuildingBlocks/
COPY ./src/Services/Flight/src/Flight/  ./Services/Flight/src/Flight/
COPY ./src/Services/Flight/src/Flight.Api/  ./Services/Flight/src/Flight.Api/

# Build project with Release configuration
# and no restore, as we did it already

RUN ls
RUN dotnet build  -c Release --no-restore ./Services/Flight/src/Flight.Api/Flight.Api.csproj

WORKDIR /app/Services/Flight/src/Flight.Api

# Publish project to output folder
# and no build, as we did it already
RUN dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Setup working directory for the project
WORKDIR /app
COPY --from=builder /app/Services/Flight/src/Flight.Api/out  .


ENV ASPNETCORE_URLS https://*:5003,http://*:5004
ENV ASPNETCORE_ENVIRONMENT docker

ENTRYPOINT ["dotnet", "Flight.Api.dll"]

