using AutoBogus;
using Booking.Booking.Features.CreateBooking;
using BuildingBlocks.IdsGenerator;

namespace Integration.Test.Fakes;

public sealed class FakeCreateBookingCommand : AutoFaker<CreateBookingCommand>
{
    public FakeCreateBookingCommand()
    {
        RuleFor(r => r.Id, _ => SnowFlakIdGenerator.NewId());
        RuleFor(r => r.FlightId, _ => 1);
        RuleFor(r => r.PassengerId, _ => 1);
    }
}
