namespace Unit.Test.Fakes;

using global::Flight.Aircrafts.ValueObjects;

public static class FakeFlightCreate
{
    public static global::Flight.Flights.Models.Flight Generate()
    {
        var command = new FakeCreateFlightCommand().Generate();

        return global::Flight.Flights.Models.Flight.Create(command.Id, command.FlightNumber,
            AircraftId.Of(command.AircraftId), command.DepartureAirportId, command.DepartureDate,
            command.ArriveDate, command.ArriveAirportId, command.DurationMinutes,
            command.FlightDate, command.Status, command.Price);
    }
}
