FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
WORKDIR /

COPY ./.editorconfig ./
COPY ./global.json ./
COPY ./Directory.Build.props ./

# Setup working directory for the project
COPY ./building-blocks/BuildingBlocks.csproj ./building-blocks/
COPY ./3-microservices-architecture-style/src/ApiGateway/src/ApiGateway.csproj ./3-microservices-architecture-style/src/ApiGateway/src/


# Restore nuget packages
RUN --mount=type=cache,id=gateway_nuget,target=/root/.nuget/packages \
    dotnet restore ./3-microservices-architecture-style/src/ApiGateway/src/ApiGateway.csproj

# Copy project files
COPY ./building-blocks ./building-blocks/
COPY ./3-microservices-architecture-style/src/ApiGateway/src  ./3-microservices-architecture-style/src/ApiGateway/src/

# Build project with Release configuration
# and no restore, as we did it already

RUN ls
RUN --mount=type=cache,id=gateway_nuget,target=/root/.nuget/packages \
    dotnet build  -c Release --no-restore ./3-microservices-architecture-style/src/ApiGateway/src/ApiGateway.csproj

WORKDIR /3-microservices-architecture-style/src/ApiGateway/src

# Publish project to output folder
# and no build, as we did it already
RUN --mount=type=cache,id=gateway_nuget,target=/root/.nuget/packages \
    dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Setup working directory for the project
WORKDIR /
COPY --from=builder /3-microservices-architecture-style/src/ApiGateway/src/out  .

ENV ASPNETCORE_URLS https://*:443, http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "ApiGateway.dll"]

