namespace Booking.Booking.Models;

using ValueObjects;

public class BookingReadModel
{
    public required Guid Id { get; init; }
    public required Guid BookId { get; init; }
    public required Trip Trip { get; init; }
    public required PassengerInfo PassengerInfo { get; init; }
    public required bool IsDeleted { get; init; }
}
