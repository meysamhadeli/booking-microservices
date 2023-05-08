namespace Integration.Test.Fakes;

using AutoBogus;
using global::Passenger.Passengers.Enums;
using global::Passenger.Passengers.Features.CompletingRegisterPassenger.V1;
using MassTransit;

public class FakeCompleteRegisterPassengerMongoCommand: AutoFaker<CompleteRegisterPassengerMongoCommand>
{
    public FakeCompleteRegisterPassengerMongoCommand()
    {
        RuleFor(r => r.Id,  _ => NewId.NextGuid());
        RuleFor(r => r.Name, _ => "Sam");
        RuleFor(r => r.PassportNumber, _ => "123456789");
        RuleFor(r => r.Age, _ => 30);
        RuleFor(r => r.IsDeleted, _ => false);
        RuleFor(r => r.PassengerType, _ => PassengerType.Male);
    }
}
