namespace Flight.Aircrafts.ValueObjects;

using System;
using Flight.Aircrafts.Exceptions;

public record AircraftId
{
    public Guid Value { get; }

    private AircraftId(Guid value)
    {
        Value = value;
    }

    public static AircraftId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidAircraftIdException(value);
        }

        return new AircraftId(value);
    }

    public static implicit operator Guid(AircraftId aircraftId)
    {
        return aircraftId.Value;
    }
}
