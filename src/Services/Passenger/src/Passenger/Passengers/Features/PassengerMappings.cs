using Mapster;

namespace Passenger.Passengers.Features;

using CompletingRegisterPassenger.V1;
using MassTransit;
using Models;

public class PassengerMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CompleteRegisterPassengerMongoCommand, PassengerReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.PassengerId, s => s.Id);

        config.NewConfig<CompleteRegisterPassengerRequestDto, CompleteRegisterPassenger>()
            .ConstructUsing(x => new CompleteRegisterPassenger(x.PassportNumber, x.PassengerType, x.Age));
    }
}
