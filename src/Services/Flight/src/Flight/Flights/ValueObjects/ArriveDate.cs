namespace Flight.Flights.ValueObjects;
using System;
using Flight.Flights.Exceptions;

public record ArriveDate
{
    public DateTime Value { get; }

    private ArriveDate(DateTime value)
    {
        if (value == null)
        {
            throw new InvalidArriveDateExceptions(value);
        }
        Value = value;
    }

    public static ArriveDate Of(DateTime value)
    {
        return new ArriveDate(value);
    }

    public static implicit operator DateTime(ArriveDate arriveDate)
    {
        return arriveDate.Value;
    }
}
