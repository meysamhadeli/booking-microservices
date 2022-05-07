using Flight.Seats.Dtos;
using Flight.Seats.Models;
using Mapster;

namespace Flight.Seats.Features;

public class SeatMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Seat, SeatResponseDto>();
    }
}

