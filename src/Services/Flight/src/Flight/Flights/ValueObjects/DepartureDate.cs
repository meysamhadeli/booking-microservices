namespace Flight.Flights.ValueObjects;
using System;
using Flight.Flights.Exceptions;


public record DepartureDate
{
    public DateTime Value { get; }

    private DepartureDate(DateTime value)
    {
        if (value == null)
        {
            throw new InvalidDepartureDateExceptions(value);
        }
        Value = value;
    }

    public static DepartureDate Of(DateTime value)
    {
        return new DepartureDate(value);
    }

    public static implicit operator DateTime(DepartureDate departureDate)
    {
        return departureDate.Value;
    }
}
