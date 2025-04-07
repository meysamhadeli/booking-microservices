using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Flights.Exceptions;

public class FlightNotFountException : NotFoundException
{
    public FlightNotFountException() : base("Flight not found!")
    {
    }
}
