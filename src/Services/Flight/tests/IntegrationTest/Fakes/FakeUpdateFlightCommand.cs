using AutoBogus;
using Flight.Flights.Features.UpdateFlight;
using Flight.Flights.Features.UpdateFlight.Commands.V1;

namespace Integration.Test.Fakes;

public class FakeUpdateFlightCommand : AutoFaker<UpdateFlightCommand>
{
    public FakeUpdateFlightCommand(global::Flight.Flights.Models.Flight flight)
    {
        RuleFor(r => r.Id, _ => flight.Id);
        RuleFor(r => r.DepartureAirportId, _ => flight.DepartureAirportId);
        RuleFor(r => r.ArriveAirportId, _ => flight.ArriveAirportId);
        RuleFor(r => r.AircraftId, _ => flight.AircraftId);
        RuleFor(r => r.FlightNumber, _ => "12UU");
        RuleFor(r => r.Price, _ => 800);
        RuleFor(r => r.Status, _ => flight.Status);
        RuleFor(r => r.ArriveDate, _ => flight.ArriveDate);
    }
}
