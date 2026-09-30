using Woo.Core.Exception;

namespace Flight.Aircrafts.Exceptions;

public class InvalidAircraftIdException : DomainException
{
    public InvalidAircraftIdException(Guid aircraftId)
        : base($"AircraftId: '{aircraftId}' is invalid.") { }
}
