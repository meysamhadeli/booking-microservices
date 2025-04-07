using BookingMonolith.Flight.Flights.Exceptions;

namespace BookingMonolith.Flight.Flights.ValueObjects;

public record FlightId
{
    public Guid Value { get; }

    private FlightId(Guid value)
    {
        Value = value;
    }

    public static FlightId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidFlightIdException(value);
        }

        return new FlightId(value);
    }

    public static implicit operator Guid(FlightId flightId)
    {
        return flightId.Value;
    }
}
