using SmartCharging.Infrastructure.Exceptions;

namespace BookingMonolith.Booking.Bookings.Exceptions;

public class InvalidDepartureAirportIdException : DomainException
{
    public InvalidDepartureAirportIdException(Guid departureAirportId)
        : base($"departureAirportId: '{departureAirportId}' is invalid.")
    {
    }
}
