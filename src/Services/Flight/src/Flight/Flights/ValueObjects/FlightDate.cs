namespace Flight.Flights.ValueObjects;
using System;
using Flight.Flights.Exceptions;

public record FlightDate
{
    public DateTime Value { get; }

    private FlightDate(DateTime value)
    {
        Value = value;
    }

    public static FlightDate Of(DateTime value)
    {
        if (value == default)
        {
            throw new InvalidFlightDateException(value);
        }

        return new FlightDate(value);
    }

    public static implicit operator DateTime(FlightDate flightDate)
    {
        return flightDate.Value;
    }
}
