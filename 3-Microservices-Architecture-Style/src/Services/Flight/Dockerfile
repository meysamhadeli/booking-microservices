FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
WORKDIR /

COPY ./.editorconfig ./
COPY ./global.json ./
COPY ./Directory.Build.props ./

# Setup working directory for the project
COPY ./BuildingBlocks/BuildingBlocks.csproj ./BuildingBlocks/
COPY ./3-Microservices-Architecture-Style/src/Services/Flight/src/Flight/Flight.csproj ./3-Microservices-Architecture-Style/src/Services/Flight/src/Flight/
COPY ./3-Microservices-Architecture-Style/src/Services/Flight/src/Flight.Api/Flight.Api.csproj ./3-Microservices-Architecture-Style/src/Services/Flight/src/Flight.Api/


# Restore nuget packages
RUN dotnet restore ./3-Microservices-Architecture-Style/src/Services/Flight/src/Flight.Api/Flight.Api.csproj

# Copy project files
COPY ./BuildingBlocks ./BuildingBlocks/
COPY ./3-Microservices-Architecture-Style/src/Services/Flight/src/Flight/  ./3-Microservices-Architecture-Style/src/Services/Flight/src/Flight/
COPY ./3-Microservices-Architecture-Style/src/Services/Flight/src/Flight.Api/  ./3-Microservices-Architecture-Style/src/Services/Flight/src/Flight.Api/

# Build project with Release configuration
# and no restore, as we did it already

RUN ls
RUN dotnet build  -c Release --no-restore ./3-Microservices-Architecture-Style/src/Services/Flight/src/Flight.Api/Flight.Api.csproj

WORKDIR /3-Microservices-Architecture-Style/src/Services/Flight/src/Flight.Api

# Publish project to output folder
# and no build, as we did it already
RUN dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Setup working directory for the project
WORKDIR /
COPY --from=builder /3-Microservices-Architecture-Style/src/Services/Flight/src/Flight.Api/out  .


ENV ASPNETCORE_URLS https://*:443, http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Flight.Api.dll"]

