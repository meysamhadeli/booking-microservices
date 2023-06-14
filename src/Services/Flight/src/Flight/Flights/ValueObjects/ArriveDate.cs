namespace Flight.Flights.ValueObjects;
using System;
using Flight.Flights.Exceptions;

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
            throw new InvalidArriveDateExceptions(value);
        }

        return new ArriveDate(value);
    }

    public static implicit operator DateTime(ArriveDate arriveDate)
    {
        return arriveDate.Value;
    }
}
