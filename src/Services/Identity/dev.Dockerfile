FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

# Setup working directory for the project
WORKDIR /
COPY ./src/BuildingBlocks/BuildingBlocks.csproj ./BuildingBlocks/
COPY ./src/Services/Identity/src/Identity/Identity.csproj ./Services/Identity/src/Identity/
COPY ./src/Services/Identity/src/Identity.Api/Identity.Api.csproj ./Services/Identity/src/Identity.Api/

# Restore nuget packages
RUN --mount=type=cache,id=identity_nuget,target=/root/.nuget/packages \
    dotnet restore ./Services/Identity/src/Identity.Api/Identity.Api.csproj

# Copy project files
COPY ./src/BuildingBlocks ./BuildingBlocks/
COPY ./src/Services/Identity/src/Identity/  ./Services/Identity/src/Identity/
COPY ./src/Services/Identity/src/Identity.Api/  ./Services/Identity/src/Identity.Api/

# Build project with Release configuration
# and no restore, as we did it already

RUN ls
RUN --mount=type=cache,id=identity_nuget,target=/root/.nuget/packages \
    dotnet build  -c Release --no-restore ./Services/Identity/src/Identity.Api/Identity.Api.csproj

WORKDIR /Services/Identity/src/Identity.Api

# Publish project to output folder
# and no build, as we did it already
RUN --mount=type=cache,id=identity_nuget,target=/root/.nuget/packages \
   dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Setup working directory for the project
WORKDIR /
COPY --from=builder /Services/Identity/src/Identity.Api/out  .

ENV ASPNETCORE_URLS https://*:443, http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Identity.Api.dll"]

