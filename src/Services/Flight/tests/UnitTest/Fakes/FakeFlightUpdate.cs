namespace Unit.Test.Fakes;

using global::Flight.Flights.Models;

public static class FakeFlightUpdate
{
    public static void Generate(Flight flight)
    {
        flight.Update(flight.Id, flight.FlightNumber, flight.AircraftId, flight.DepartureAirportId, flight.DepartureDate,
            flight.ArriveDate, flight.ArriveAirportId, flight.DurationMinutes, flight.FlightDate, flight.Status, 1000, flight.IsDeleted);
    }
}
