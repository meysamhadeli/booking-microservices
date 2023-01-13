using System;
using AutoBogus;
using Flight;
using Google.Protobuf.WellKnownTypes;

namespace Integration.Test.Fakes;

public class FakeFlightResponse : AutoFaker<FlightResponse>
{
    public FakeFlightResponse()
    {
        RuleFor(r => r.Id, _ => 1);
        RuleFor(r => r.Price, _ => 100);
        RuleFor(r => r.Status, _ => FlightStatus.Completed);
        RuleFor(r => r.AircraftId, _ => 1);
        RuleFor(r => r.ArriveAirportId, _ => 1);
        RuleFor(r => r.ArriveDate, _ => DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc).ToTimestamp());
        RuleFor(r => r.DepartureDate, _ => DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc).ToTimestamp());
        RuleFor(r => r.FlightDate, _ => DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc).ToTimestamp());
        RuleFor(r => r.FlightNumber, r => r.Random.Number(1000, 2000).ToString());
        RuleFor(r => r.DepartureAirportId, _ => 2);
    }
}
