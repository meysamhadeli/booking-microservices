using BookingMonolith.Passenger.Exceptions;

namespace BookingMonolith.Passenger.Passengers.ValueObjects;

public record PassengerId
{
    public Guid Value { get; }

    private PassengerId(Guid value)
    {
        Value = value;
    }

    public static PassengerId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidPassengerIdException(value);
        }

        return new PassengerId(value);
    }

    public static implicit operator Guid(PassengerId passengerId)
    {
        return passengerId.Value;
    }
}
