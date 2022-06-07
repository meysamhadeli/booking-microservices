using AutoBogus;
using Passenger.Passengers.Dtos;

namespace Integration.Test.Fakes;

public class FakePassengerResponseDto : AutoFaker<PassengerResponseDto>
{
    public FakePassengerResponseDto(long id)
    {
        RuleFor(r => r.Id,  _ => id);
    }
}
