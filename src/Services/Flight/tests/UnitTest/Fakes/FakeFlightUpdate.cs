namespace Unit.Test.Fakes;

using global::Flight.Flights.Models;

public static class FakeFlightUpdate
{
    public static void Generate(Flight flight)
    {
        flight.Update(flight.Id, flight.FlightNumber, 3, flight.DepartureAirportId, flight.DepartureDate,
            flight.ArriveDate, 3, flight.DurationMinutes, flight.FlightDate, flight.Status, flight.Price, flight.IsDeleted);;
    }
}
