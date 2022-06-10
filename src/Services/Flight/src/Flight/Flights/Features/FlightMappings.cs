using AutoMapper;
using Flight.Flights.Dtos;
using Mapster;

namespace Flight.Flights.Features;

public class FlightMappings : Profile
{
    public void Register(TypeAdapterConfig config)
    {
         config.NewConfig<Models.Flight, FlightResponseDto>();
     }
}
