FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /src

# Setup working directory for the project
WORKDIR /src
COPY ./src/BuildingBlocks/BuildingBlocks.csproj ./BuildingBlocks/
COPY ./src/ApiGateway/src/ApiGateway.csproj ./ApiGateway/src/


# Restore nuget packages
RUN dotnet restore ./ApiGateway/src/ApiGateway.csproj

# Copy project files
COPY ./src/BuildingBlocks ./BuildingBlocks/
COPY ./src/ApiGateway/src  ./ApiGateway/src/

# Build project with Release configuration
# and no restore, as we did it already

RUN ls
RUN dotnet build  -c Release --no-restore ./ApiGateway/src/ApiGateway.csproj

WORKDIR /src/ApiGateway/src

# Publish project to output folder
# and no build, as we did it already
RUN dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Setup working directory for the project
WORKDIR /app
COPY --from=builder /src/ApiGateway/src/out  .

ENV ASPNETCORE_URLS https://*:443, http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "ApiGateway.dll"]

