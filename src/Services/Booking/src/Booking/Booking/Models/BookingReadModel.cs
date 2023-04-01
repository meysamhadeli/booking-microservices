namespace Booking.Booking.Models;

using ValueObjects;

public class BookingReadModel
{
    public Guid Id { get; init; }
    public Guid BookId { get; init; }
    public Trip Trip { get; init; }
    public PassengerInfo PassengerInfo { get; init; }
    public bool IsDeleted { get; init; }
}
