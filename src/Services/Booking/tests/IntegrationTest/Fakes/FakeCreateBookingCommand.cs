using AutoBogus;
using BuildingBlocks.IdsGenerator;

namespace Integration.Test.Fakes;

using global::Booking.Booking.Features.CreatingBook.Commands.V1;

public sealed class FakeCreateBookingCommand : AutoFaker<CreateBooking>
{
    public FakeCreateBookingCommand()
    {
        RuleFor(r => r.Id, _ => SnowFlakIdGenerator.NewId());
        RuleFor(r => r.FlightId, _ => 1);
        RuleFor(r => r.PassengerId, _ => 1);
    }
}
