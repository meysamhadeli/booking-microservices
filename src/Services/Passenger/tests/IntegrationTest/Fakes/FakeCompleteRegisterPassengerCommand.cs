using AutoBogus;
using BuildingBlocks.IdsGenerator;
using Passenger.Passengers.Enums;
using Passenger.Passengers.Features.CompleteRegisterPassenger;
using Passenger.Passengers.Models;

namespace Integration.Test.Fakes;

public sealed class FakeCompleteRegisterPassengerCommand : AutoFaker<CompleteRegisterPassengerCommand>
{
    public FakeCompleteRegisterPassengerCommand(string passportNumber)
    {
        RuleFor(r => r.Id, _ => SnowFlakIdGenerator.NewId());
        RuleFor(r => r.PassportNumber, _ => passportNumber);
        RuleFor(r => r.PassengerType, _ => PassengerType.Male);
        RuleFor(r => r.Age, _ => 30);
    }
}

