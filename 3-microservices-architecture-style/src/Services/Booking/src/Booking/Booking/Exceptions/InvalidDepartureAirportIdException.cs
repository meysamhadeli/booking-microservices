using SmartCharging.Infrastructure.Exceptions;

namespace Booking.Booking.Exceptions;

public class InvalidDepartureAirportIdException : DomainException
{
    public InvalidDepartureAirportIdException(Guid departureAirportId)
        : base($"departureAirportId: '{departureAirportId}' is invalid.")
    {
    }
}
