FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
WORKDIR /

COPY ./.editorconfig ./
COPY ./global.json ./
COPY ./Directory.Build.props ./

# Setup working directory for the project
COPY ./building-blocks/BuildingBlocks.csproj ./building-blocks/
COPY ./src/Services/Booking/src/Booking/Booking.csproj ./src/Services/Booking/src/Booking/
COPY ./src/Services/Booking/src/Booking.Api/Booking.Api.csproj ./src/Services/Booking/src/Booking.Api/


# Restore nuget packages
RUN  dotnet restore ./src/Services/Booking/src/Booking.Api/Booking.Api.csproj

# Copy project files
COPY ./building-blocks ./building-blocks/
COPY ./src/Services/Booking/src/Booking/  ./src/Services/Booking/src/Booking/
COPY ./src/Services/Booking/src/Booking.Api/  ./src/Services/Booking/src/Booking.Api/

# Build project with Release configuration
# and no restore, as we did it already

RUN ls
RUN  dotnet build  -c Release --no-restore ./src/Services/Booking/src/Booking.Api/Booking.Api.csproj

WORKDIR /src/Services/Booking/src/Booking.Api

# Publish project to output folder
# and no build, as we did it already
RUN dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Setup working directory for the project
WORKDIR /
COPY --from=builder /src/Services/Booking/src/Booking.Api/out  .

ENV ASPNETCORE_URLS https://*:443, http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Booking.Api.dll"]

