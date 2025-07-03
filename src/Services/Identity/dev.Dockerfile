FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder

# Setup working directory for the project
WORKDIR /

COPY ./.editorconfig ./
COPY ./global.json ./
COPY ./Directory.Build.props ./

COPY ./building-blocks/BuildingBlocks.csproj ./building-blocks/
COPY ./src/Services/Identity/src/Identity/Identity.csproj ./src/Services/Identity/src/Identity/
COPY ./src/Services/Identity/src/Identity.Api/Identity.Api.csproj ./src/Services/Identity/src/Identity.Api/

# Restore nuget packages
RUN --mount=type=cache,id=identity_nuget,target=/root/.nuget/packages \
    dotnet restore ./src/Services/Identity/src/Identity.Api/Identity.Api.csproj

# Copy project files
COPY ./building-blocks ./building-blocks/
COPY ./src/Services/Identity/src/Identity/  ./src/Services/Identity/src/Identity/
COPY ./src/Services/Identity/src/Identity.Api/  ./src/Services/Identity/src/Identity.Api/

# Build project with Release configuration
# and no restore, as we did it already

RUN ls
RUN --mount=type=cache,id=identity_nuget,target=/root/.nuget/packages \
    dotnet build  -c Release --no-restore ./src/Services/Identity/src/Identity.Api/Identity.Api.csproj

WORKDIR /src/Services/Identity/src/Identity.Api

# Publish project to output folder
# and no build, as we did it already
RUN --mount=type=cache,id=identity_nuget,target=/root/.nuget/packages \
   dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Setup working directory for the project
WORKDIR /
COPY --from=builder /src/Services/Identity/src/Identity.Api/out  .

ENV ASPNETCORE_URLS https://*:443, http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Identity.Api.dll"]

