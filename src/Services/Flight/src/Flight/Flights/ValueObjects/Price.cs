namespace Flight.Flights.ValueObjects;
using Flight.Flights.Exceptions;

public class Price
{
    public decimal Value { get; }

    public Price(decimal value)
    {
        if (value < 0)
        {
            throw new InvalidPriceException();
        }
        Value = value;
    }

    public static Price Of(decimal value)
    {
        return new Price(value);
    }

    public static implicit operator decimal(Price price)
    {
        return price.Value;
    }
}
