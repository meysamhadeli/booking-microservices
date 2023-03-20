using AutoBogus;
using BuildingBlocks.IdsGenerator;
using Flight.Flights.Enums;

namespace EndToEnd.Test.Fakes;

using global::Flight.Flights.Features.CreatingFlight.V1;

public sealed class FakeCreateFlightCommand : AutoFaker<CreateFlight>
{
    public FakeCreateFlightCommand()
    {
        RuleFor(r => r.Id, _ => SnowFlakIdGenerator.NewId());
        RuleFor(r => r.FlightNumber, r => "12FF");
        RuleFor(r => r.DepartureAirportId, _ => 1);
        RuleFor(r => r.ArriveAirportId, _ => 2);
        RuleFor(r => r.Status, _ => FlightStatus.Flying);
        RuleFor(r => r.AircraftId, _ => 1);
    }
}
