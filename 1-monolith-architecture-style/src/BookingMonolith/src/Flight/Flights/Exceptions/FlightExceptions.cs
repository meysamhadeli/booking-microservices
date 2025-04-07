using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Flights.Exceptions;

public class FlightExceptions : BadRequestException
{
    public FlightExceptions(DateTime departureDate, DateTime arriveDate) :
        base($"Departure date: '{departureDate}' must be before arrive date: '{arriveDate}'.")
    { }

    public FlightExceptions(DateTime flightDate) :
       base($"Flight date: '{flightDate}' must be between departure and arrive dates.")
    { }
}
