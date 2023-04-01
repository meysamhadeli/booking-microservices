using AutoBogus;

namespace Integration.Test.Fakes;

using System;
using global::Booking.Booking.Features.CreatingBook.Commands.V1;
using MassTransit;

public sealed class FakeCreateBookingCommand : AutoFaker<CreateBooking>
{
    public FakeCreateBookingCommand()
    {
        RuleFor(r => r.Id, _ => NewId.NextGuid());
        RuleFor(r => r.FlightId, _ => new Guid("3c5c0000-97c6-fc34-2eb9-08db322230c9"));
        RuleFor(r => r.PassengerId, _ => new Guid("4c5c8888-97c6-fc34-2eb9-18db322230c1"));
    }
}
