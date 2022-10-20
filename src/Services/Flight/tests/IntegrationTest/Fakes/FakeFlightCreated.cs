using Flight.Flights.Features.CreateFlight;
using Flight.Flights.Features.CreateFlight.Commands.V1;

namespace Integration.Test.Fakes;

public static class FakeFlightCreated
{
    public static global::Flight.Flights.Models.Flight Generate(CreateFlightCommand command)
    {
        return global::Flight.Flights.Models.Flight.Create(command.Id, command.FlightNumber,
            command.AircraftId, command.DepartureAirportId, command.DepartureDate,
            command.ArriveDate, command.ArriveAirportId, command.DurationMinutes,
            command.FlightDate, command.Status, command.Price);
    }
}
