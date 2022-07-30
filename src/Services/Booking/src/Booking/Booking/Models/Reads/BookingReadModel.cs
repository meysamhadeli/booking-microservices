using Booking.Booking.Models.ValueObjects;

namespace Booking.Booking.Models.Reads;

public class BookingReadModel
{
    public long Id { get; init; }
    public long BookId { get; init; }
    public Trip Trip { get; init; }
    public PassengerInfo PassengerInfo { get; init; }
    public bool IsDeleted { get; init; }
}
