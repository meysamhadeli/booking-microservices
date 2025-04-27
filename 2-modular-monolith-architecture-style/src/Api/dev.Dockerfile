FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
WORKDIR /

COPY ./.editorconfig ./
COPY ./global.json ./
COPY ./Directory.Build.props ./

# Setup working directory for the project
COPY ./building-blocks/BuildingBlocks.csproj ./building-blocks/
COPY ./2-modular-monolith-architecture-style/src/Modules/Booking/src/Booking.csproj ./2-modular-monolith-architecture-style/src/Modules/Booking/src/
COPY ./2-modular-monolith-architecture-style/src/Modules/Flight/src/Flight.csproj ./2-modular-monolith-architecture-style/src/Modules/Flight/src/
COPY ./2-modular-monolith-architecture-style/src/Modules/Identity/src/Identity.csproj ./2-modular-monolith-architecture-style/src/Modules/Identity/src/
COPY ./2-modular-monolith-architecture-style/src/Modules/Passenger/src/Passenger.csproj ./2-modular-monolith-architecture-style/src/Modules/Passenger/src/
COPY ./2-modular-monolith-architecture-style/src/Api/src/Api.csproj ./2-modular-monolith-architecture-style/src/Api/src/

# Restore nuget packages
RUN --mount=type=cache,id=booking_nuget,target=/root/.nuget/packages \
    dotnet restore ./2-modular-monolith-architecture-style/src/Api/src/Api.csproj

# Copy project files
COPY ./building-blocks ./building-blocks/
COPY ./2-modular-monolith-architecture-style/src/Modules/Booking/src/ ./2-modular-monolith-architecture-style/src/Modules/Booking/src/
COPY ./2-modular-monolith-architecture-style/src/Modules/Flight/src/ ./2-modular-monolith-architecture-style/src/Modules/Flight/src/
COPY ./2-modular-monolith-architecture-style/src/Modules/Identity/src/ ./2-modular-monolith-architecture-style/src/Modules/Identity/src/
COPY ./2-modular-monolith-architecture-style/src/Modules/Passenger/src/ ./2-modular-monolith-architecture-style/src/Modules/Passenger/src/
COPY ./2-modular-monolith-architecture-style/src/Api/src/ ./2-modular-monolith-architecture-style/src/Api/src/

# Build project with Release configuration
# and no restore, as we did it already
RUN ls
RUN --mount=type=cache,id=booking_nuget,target=/root/.nuget/packages\
    dotnet build  -c Release --no-restore ./2-modular-monolith-architecture-style/src/Api/src/Api.csproj

WORKDIR /2-modular-monolith-architecture-style/src/Api/src

# Publish project to output folder
# and no build, as we did it already
RUN --mount=type=cache,id=booking_nuget,target=/root/.nuget/packages\
    dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Setup working directory for the project
WORKDIR /
COPY --from=builder /2-modular-monolith-architecture-style/src/Api/src/out  .

ENV ASPNETCORE_URLS="https://*:443, http://*:80"
ENV ASPNETCORE_ENVIRONMENT=docker

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Api.dll"]

