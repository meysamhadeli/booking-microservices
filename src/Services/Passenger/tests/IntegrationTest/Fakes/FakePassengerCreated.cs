using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.IdsGenerator;

namespace Integration.Test.Fakes;

public static class FakePassengerCreated
{
    public static global::Passenger.Passengers.Models.Passenger Generate(UserCreated userCreated)
    {
        return global::Passenger.Passengers.Models.Passenger.Create(1000, userCreated.Name, userCreated.PassportNumber);
    }
}
