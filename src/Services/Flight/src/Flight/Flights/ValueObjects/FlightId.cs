namespace Flight.Flights.ValueObjects;
using System;
using Exceptions;

public record FlightId
{
    public Guid Value { get; }

    private FlightId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidFlightIdExceptions(value);
        }

        Value = value;
    }

    public static FlightId Of(Guid value)
    {
        return new FlightId(value);
    }

    public static implicit operator Guid(FlightId flightId)
    {
        return flightId.Value;
    }
}
