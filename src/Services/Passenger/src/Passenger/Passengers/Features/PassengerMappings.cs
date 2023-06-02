using Mapster;

namespace Passenger.Passengers.Features;

using CompletingRegisterPassenger.V1;
using Dtos;
using MassTransit;
using Models;
using ValueObjects;

public class PassengerMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CompleteRegisterPassengerMongoCommand, PassengerReadModel>()
        .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.PassengerId, s => PassengerId.Of(s.Id));

        config.NewConfig<CompleteRegisterPassengerRequestDto, CompleteRegisterPassenger>()
            .ConstructUsing(x => new CompleteRegisterPassenger(x.PassportNumber, x.PassengerType, x.Age));

        config.NewConfig<PassengerReadModel, PassengerDto>()
            .ConstructUsing(x => new PassengerDto(x.PassengerId, x.Name, x.PassportNumber, x.PassengerType, x.Age));

        config.NewConfig<Passenger, PassengerDto>()
            .ConstructUsing(x => new PassengerDto(x.Id.Value, x.Name.Value, x.PassportNumber.Value, x.PassengerType, x.Age.Value));
    }
}
