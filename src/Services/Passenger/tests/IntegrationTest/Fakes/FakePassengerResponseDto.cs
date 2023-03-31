using AutoBogus;

namespace Integration.Test.Fakes;

using global::Passenger;
using MassTransit;

public class FakePassengerResponse : AutoFaker<PassengerResponse>
{
    public FakePassengerResponse()
    {
        RuleFor(r => r.Id, _ => NewId.NextGuid().ToString());
    }
}
