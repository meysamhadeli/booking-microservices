using BookingMonolith.Flight.Flights.Exceptions;

namespace BookingMonolith.Flight.Flights.ValueObjects;

public class Price
{
    public decimal Value { get; }

    private Price(decimal value)
    {
        Value = value;
    }

    public static Price Of(decimal value)
    {
        if (value < 0)
        {
            throw new InvalidPriceException();
        }

        return new Price(value);
    }

    public static implicit operator decimal(Price price)
    {
        return price.Value;
    }
}
