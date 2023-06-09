namespace Flight.Flights.ValueObjects;
using System;
using Flight.Flights.Exceptions;

public record FlightDate
{
    public DateTime Value { get; }

    private FlightDate(DateTime value)
    {
        if (value == null)
        {
            throw new InvalidFlightDateExceptions(value);
        }
        Value = value;
    }

    public static FlightDate Of(DateTime value)
    {
        return new FlightDate(value);
    }

    public static implicit operator DateTime(FlightDate flightDate)
    {
        return flightDate.Value;
    }
}
