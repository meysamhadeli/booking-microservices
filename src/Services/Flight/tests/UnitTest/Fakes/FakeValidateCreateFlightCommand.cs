using System;
using AutoBogus;
using Flight.Flights.Enums;

namespace Unit.Test.Fakes;

using global::Flight.Flights.Features.CreatingFlight.V1;

public class FakeValidateCreateFlightCommand : AutoFaker<CreateFlight>
{
    public FakeValidateCreateFlightCommand()
    {
        RuleFor(r => r.Price, _ => -10);
        RuleFor(r => r.Status, _ => (FlightStatus)10);
        RuleFor(r => r.AircraftId, _ => 0);
        RuleFor(r => r.DepartureAirportId, _ => 0);
        RuleFor(r => r.ArriveAirportId, _ => 0);
        RuleFor(r => r.DurationMinutes, _ => 0);
        RuleFor(r => r.FlightDate, _ => new DateTime());
    }
}
