namespace Flight.Flights.ValueObjects;
using Exceptions;

public class DurationMinutes
{
    public decimal Value { get; }

    public DurationMinutes(decimal value)
    {
        if (value < 0)
        {
            throw new InvalidDurationException();
        }
        Value = value;
    }

    public static DurationMinutes Of(decimal value)
    {
        return new DurationMinutes(value);
    }

    public static implicit operator decimal(DurationMinutes duration)
    {
        return duration.Value;
    }

    public static explicit operator DurationMinutes(decimal value)
    {
        return new DurationMinutes(value);
    }
}
