namespace Booking.Booking.Features.CreateBooking.Dtos.V1;

public record CreateBookingRequestDto(long PassengerId, long FlightId, string Description);
