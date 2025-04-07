using BookingMonolith.Booking.Bookings.Exceptions;

namespace BookingMonolith.Booking.Bookings.ValueObjects;

public record PassengerInfo
{
    public string Name { get; }

    private PassengerInfo(string name)
    {
        Name = name;
    }

    public static PassengerInfo Of(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidPassengerNameException(name);
        }

        return new PassengerInfo(name);
    }
}
