FROM mcr.microsoft.com/dotnet/sdk:7.0 AS builder
WORKDIR /

# Setup working directory for the project
COPY ./src/BuildingBlocks/BuildingBlocks.csproj ./BuildingBlocks/
COPY ./src/Services/Booking/src/Booking/Booking.csproj ./Services/Booking/src/Booking/
COPY ./src/Services/Booking/src/Booking.Api/Booking.Api.csproj ./Services/Booking/src/Booking.Api/


# Restore nuget packages
RUN --mount=type=cache,id=booking_nuget,target=/root/.nuget/packages \
    dotnet restore ./Services/Booking/src/Booking.Api/Booking.Api.csproj

# Copy project files
COPY ./src/BuildingBlocks ./BuildingBlocks/
COPY ./src/Services/Booking/src/Booking/  ./Services/Booking/src/Booking/
COPY ./src/Services/Booking/src/Booking.Api/  ./Services/Booking/src/Booking.Api/

# Build project with Release configuration
# and no restore, as we did it already

RUN ls
RUN --mount=type=cache,id=booking_nuget,target=/root/.nuget/packages\
    dotnet build  -c Release --no-restore ./Services/Booking/src/Booking.Api/Booking.Api.csproj

WORKDIR /Services/Booking/src/Booking.Api

# Publish project to output folder
# and no build, as we did it already
RUN --mount=type=cache,id=booking_nuget,target=/root/.nuget/packages\
    dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0

# Setup working directory for the project
WORKDIR /
COPY --from=builder /Services/Booking/src/Booking.Api/out  .

ENV ASPNETCORE_URLS https://*:443, http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Booking.Api.dll"]

