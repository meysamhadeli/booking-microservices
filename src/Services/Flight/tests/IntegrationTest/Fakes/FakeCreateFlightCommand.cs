using AutoBogus;
using BuildingBlocks.IdsGenerator;
using Flight.Flights.Enums;
using Flight.Flights.Features.CreateFlight.Commands.V1;

namespace Integration.Test.Fakes;

public sealed class FakeCreateFlightCommand : AutoFaker<CreateFlightCommand>
{
    public FakeCreateFlightCommand()
    {
        RuleFor(r => r.Id, _ => SnowFlakIdGenerator.NewId());
        RuleFor(r => r.FlightNumber, r => r.Random.Number(1000, 2000).ToString());
        RuleFor(r => r.DepartureAirportId, _ => 1);
        RuleFor(r => r.ArriveAirportId, _ => 2);
        RuleFor(r => r.Status, _ => FlightStatus.Flying);
        RuleFor(r => r.AircraftId, _ => 1);
    }
}
