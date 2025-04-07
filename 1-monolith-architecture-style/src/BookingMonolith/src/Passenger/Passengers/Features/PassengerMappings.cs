using BookingMonolith.Passenger.Passengers.Dtos;
using BookingMonolith.Passenger.Passengers.Features.CompletingRegisterPassenger.V1;
using BookingMonolith.Passenger.Passengers.Models;
using BookingMonolith.Passenger.Passengers.ValueObjects;
using Mapster;
using MassTransit;

namespace BookingMonolith.Passenger.Passengers.Features;

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

        config.NewConfig<Models.Passenger, PassengerDto>()
            .ConstructUsing(x => new PassengerDto(x.Id.Value, x.Name.Value, x.PassportNumber.Value, x.PassengerType, x.Age.Value));
    }
}
