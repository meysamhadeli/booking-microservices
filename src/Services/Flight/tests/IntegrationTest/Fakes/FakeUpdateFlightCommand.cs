using AutoBogus;
using Flight.Flights.Features.UpdateFlight;

namespace Integration.Test.Fakes;

public class FakeUpdateFlightCommand : AutoFaker<UpdateFlightCommand>
{
    public FakeUpdateFlightCommand(long id)
    {
        RuleFor(r => r.Id, _ => id);
        RuleFor(r => r.DepartureAirportId, _ => 2);
        RuleFor(r => r.ArriveAirportId, _ => 1);
        RuleFor(r => r.AircraftId, _ => 2);
        RuleFor(r => r.FlightNumber, _ => "12BB");
        RuleFor(r => r.Price, _ => 800);
    }
}
