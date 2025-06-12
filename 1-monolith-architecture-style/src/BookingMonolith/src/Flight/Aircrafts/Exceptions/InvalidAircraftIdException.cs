using SmartCharging.Infrastructure.Exceptions;

namespace BookingMonolith.Flight.Aircrafts.Exceptions;

public class InvalidAircraftIdException : DomainException
{
    public InvalidAircraftIdException(Guid aircraftId)
        : base($"AircraftId: '{aircraftId}' is invalid.")
    {
    }
}
