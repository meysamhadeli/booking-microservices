namespace Flight.Aircrafts.Models.ValueObjects;
using System;
using Exceptions;

public record AircraftId
{
    public Guid Value { get; }

    private AircraftId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidAircraftIdExceptions(value);
        }

        Value = value;
    }

    public static AircraftId Of(Guid value)
    {
        return new AircraftId(value);
    }

    public static implicit operator Guid(AircraftId aircraftId)
    {
        return aircraftId.Value;
    }
}
