using BookingMonolith.Booking.Bookings.ValueObjects;

namespace BookingMonolith.Booking.Bookings.Models;

public class BookingReadModel
{
    public required Guid Id { get; init; }
    public required Guid BookId { get; init; }
    public required Trip Trip { get; init; }
    public required PassengerInfo PassengerInfo { get; init; }
    public required bool IsDeleted { get; init; }
}
