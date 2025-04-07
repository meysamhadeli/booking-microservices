using BookingMonolith.Flight.Flights.Exceptions;

namespace BookingMonolith.Flight.Flights.ValueObjects;

public record ArriveDate
{
    public DateTime Value { get; }

    private ArriveDate(DateTime value)
    {
        Value = value;
    }

    public static ArriveDate Of(DateTime value)
    {
        if (value == default)
        {
            throw new InvalidArriveDateException(value);
        }

        return new ArriveDate(value);
    }

    public static implicit operator DateTime(ArriveDate arriveDate)
    {
        return arriveDate.Value;
    }
}
