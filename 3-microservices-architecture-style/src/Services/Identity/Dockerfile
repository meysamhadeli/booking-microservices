FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder

# Setup working directory for the project
WORKDIR /

COPY ./.editorconfig ./
COPY ./global.json ./
COPY ./Directory.Build.props ./

COPY ./building-blocks/BuildingBlocks.csproj ./building-blocks/
COPY ./3-microservices-architecture-style/src/Services/Identity/src/Identity/Identity.csproj ./3-microservices-architecture-style/src/Services/Identity/src/Identity/
COPY ./3-microservices-architecture-style/src/Services/Identity/src/Identity.Api/Identity.Api.csproj ./3-microservices-architecture-style/src/Services/Identity/src/Identity.Api/

# Restore nuget packages
RUN dotnet restore ./3-microservices-architecture-style/src/Services/Identity/src/Identity.Api/Identity.Api.csproj

# Copy project files
COPY ./building-blocks ./building-blocks/
COPY ./3-microservices-architecture-style/src/Services/Identity/src/Identity/  ./3-microservices-architecture-style/src/Services/Identity/src/Identity/
COPY ./3-microservices-architecture-style/src/Services/Identity/src/Identity.Api/  ./3-microservices-architecture-style/src/Services/Identity/src/Identity.Api/

# Build project with Release configuration
# and no restore, as we did it already

RUN ls
RUN dotnet build  -c Release --no-restore ./3-microservices-architecture-style/src/Services/Identity/src/Identity.Api/Identity.Api.csproj

WORKDIR /3-microservices-architecture-style/src/Services/Identity/src/Identity.Api

# Publish project to output folder
# and no build, as we did it already
RUN dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Setup working directory for the project
WORKDIR /
COPY --from=builder /3-microservices-architecture-style/src/Services/Identity/src/Identity.Api/out  .

ENV ASPNETCORE_URLS https://*:443, http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Identity.Api.dll"]

