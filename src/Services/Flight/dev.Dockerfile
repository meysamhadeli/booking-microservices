FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
WORKDIR /

COPY ./.editorconfig ./
COPY ./global.json ./
COPY ./Directory.Build.props ./

# Setup working directory for the project
COPY ./building-blocks/BuildingBlocks.csproj ./building-blocks/
COPY ./src/Services/Flight/src/Flight/Flight.csproj ./src/Services/Flight/src/Flight/
COPY ./src/Services/Flight/src/Flight.Api/Flight.Api.csproj ./src/Services/Flight/src/Flight.Api/


# Restore nuget packages
RUN --mount=type=cache,id=flight_nuget,target=/root/.nuget/packages \
    dotnet restore ./src/Services/Flight/src/Flight.Api/Flight.Api.csproj

# Copy project files
COPY ./building-blocks ./building-blocks/
COPY ./src/Services/Flight/src/Flight/  ./src/Services/Flight/src/Flight/
COPY ./src/Services/Flight/src/Flight.Api/  ./src/Services/Flight/src/Flight.Api/

# Build project with Release configuration
# and no restore, as we did it already

RUN ls
RUN --mount=type=cache,id=flight_nuget,target=/root/.nuget/packages \
    dotnet build  -c Release --no-restore ./src/Services/Flight/src/Flight.Api/Flight.Api.csproj

WORKDIR /src/Services/Flight/src/Flight.Api

# Publish project to output folder
# and no build, as we did it already
RUN --mount=type=cache,id=flight_nuget,target=/root/.nuget/packages \
    dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Setup working directory for the project
WORKDIR /
COPY --from=builder /src/Services/Flight/src/Flight.Api/out  .


ENV ASPNETCORE_URLS https://*:443, http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Flight.Api.dll"]

