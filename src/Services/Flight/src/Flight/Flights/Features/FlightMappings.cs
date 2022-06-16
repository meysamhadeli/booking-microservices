using AutoMapper;
using Flight.Flights.Dtos;
using Flight.Flights.Features.CreateFlight.Reads;
using Flight.Flights.Models.Reads;
using Mapster;

namespace Flight.Flights.Features;

public class FlightMappings : Profile
{
    public void Register(TypeAdapterConfig config)
    {
         config.NewConfig<Models.Flight, FlightResponseDto>();
         config.NewConfig<Models.Flight, CreateFlightMongoCommand>();
         config.NewConfig<CreateFlightMongoCommand, FlightReadModel>();
    }
}
