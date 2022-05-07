FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder
WORKDIR /app

# Setup working directory for the project
WORKDIR /app
COPY ./src/BuildingBlocks/BuildingBlocks.csproj ./BuildingBlocks/
COPY ./src/Services/Identity/src/Identity/Identity.csproj ./Services/Identity/src/Identity/
COPY ./src/Services/Identity/src/Identity.Api/Identity.Api.csproj ./Services/Identity/src/Identity.Api/


# Restore nuget packages
RUN dotnet restore ./Services/Identity/src/Identity.Api/Identity.Api.csproj

# Copy project files
COPY ./src/BuildingBlocks ./BuildingBlocks/
COPY ./src/Services/Identity/src/Identity/  ./Services/Identity/src/Identity/
COPY ./src/Services/Identity/src/Identity.Api/  ./Services/Identity/src/Identity.Api/

# Build project with Release configuration
# and no restore, as we did it already

RUN ls
RUN dotnet build  -c Release --no-restore ./Services/Identity/src/Identity.Api/Identity.Api.csproj

WORKDIR /app/Services/Identity/src/Identity.Api

# Publish project to output folder
# and no build, as we did it already
RUN dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Setup working directory for the project
WORKDIR /app
COPY --from=builder /app/Services/Identity/src/Identity.Api/out  .

ENV ASPNETCORE_URLS https://*:5005, http://*:6005
ENV ASPNETCORE_ENVIRONMENT docker  

ENTRYPOINT ["dotnet", "Identity.Api.dll"]

