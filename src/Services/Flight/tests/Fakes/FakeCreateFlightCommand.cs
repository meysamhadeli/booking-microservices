using AutoBogus;
using Flight.Flights.Features.CreateFlight;

namespace Integration.Test.Fakes;

public sealed class FakeCreateFlightCommand : AutoFaker<CreateFlightCommand>
{
    public FakeCreateFlightCommand()
    {
        RuleFor(r => r.DepartureAirportId, _ => 1);
        RuleFor(r => r.ArriveAirportId, _ => 2);
        RuleFor(r => r.AircraftId, _ => 1);
    }
}
