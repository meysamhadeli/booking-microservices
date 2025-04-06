namespace Flight.Airports.ValueObjects;

using System;
using Flight.Airports.Exceptions;

public record AirportId
{
    public Guid Value { get; }

    private AirportId(Guid value)
    {
        Value = value;
    }

    public static AirportId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidAirportIdException(value);
        }

        return new AirportId(value);
    }

    public static implicit operator Guid(AirportId airportId)
    {
        return airportId.Value;
    }
}
