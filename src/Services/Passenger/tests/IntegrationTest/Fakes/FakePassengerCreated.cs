using BuildingBlocks.Contracts.EventBus.Messages;

namespace Integration.Test.Fakes;

using MassTransit;

public static class FakePassengerCreated
{
    public static global::Passenger.Passengers.Models.Passenger Generate(UserCreated userCreated)
    {
        return global::Passenger.Passengers.Models.Passenger.Create(NewId.NextGuid(), userCreated.Name, userCreated.PassportNumber);
    }
}
