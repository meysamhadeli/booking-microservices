using AutoMapper;
using Passenger.Passengers.Dtos;

namespace Passenger.Passengers.Features;

public class ReservationMappings: Profile
{
    public ReservationMappings()
    {
        CreateMap<Models.Passenger, PassengerResponseDto>();
    }
}
