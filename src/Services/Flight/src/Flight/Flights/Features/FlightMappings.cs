using Flight.Flights.Dtos;
using Mapster;

namespace Flight.Flights.Features;

public class FlightMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.Flight, FlightResponseDto>();
    }
}
