FROM mcr.microsoft.com/dotnet/sdk:7.0 AS builder
WORKDIR /

# Setup working directory for the project
COPY ./src/BuildingBlocks/BuildingBlocks.csproj ./BuildingBlocks/
COPY ./src/Services/Passenger/src/Passenger/Passenger.csproj ./Services/Passenger/src/Passenger/
COPY ./src/Services/Passenger/src/Passenger.Api/Passenger.Api.csproj ./Services/Passenger/src/Passenger.Api/


# Restore nuget packages
RUN --mount=type=cache,id=passenger_nuget,target=/root/.nuget/packages \
    dotnet restore ./Services/Passenger/src/Passenger.Api/Passenger.Api.csproj

# Copy project files
COPY ./src/BuildingBlocks ./BuildingBlocks/
COPY ./src/Services/Passenger/src/Passenger/  ./Services/Passenger/src/Passenger/
COPY ./src/Services/Passenger/src/Passenger.Api/  ./Services/Passenger/src/Passenger.Api/

# Build project with Release configuration
# and no restore, as we did it already

RUN ls
RUN --mount=type=cache,id=passenger_nuget,target=/root/.nuget/packages \
    dotnet build  -c Release --no-restore ./Services/Passenger/src/Passenger.Api/Passenger.Api.csproj

WORKDIR /Services/Passenger/src/Passenger.Api

# Publish project to output folder
# and no build, as we did it already
RUN --mount=type=cache,id=passenger_nuget,target=/root/.nuget/packages \
    dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0

# Setup working directory for the project
WORKDIR /
COPY --from=builder /Services/Passenger/src/Passenger.Api/out  .

ENV ASPNETCORE_URLS https://*:443, http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Passenger.Api.dll"]

