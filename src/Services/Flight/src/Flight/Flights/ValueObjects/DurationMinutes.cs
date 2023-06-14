namespace Flight.Flights.ValueObjects;

using Exceptions;

public class DurationMinutes
{
    public decimal Value { get; }

    private DurationMinutes(decimal value)
    {
        Value = value;
    }

    public static DurationMinutes Of(decimal value)
    {
        if (value < 0)
        {
            throw new InvalidDurationException();
        }

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
