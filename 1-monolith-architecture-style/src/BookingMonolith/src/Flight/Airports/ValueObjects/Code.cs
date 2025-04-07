using BookingMonolith.Flight.Airports.Exceptions;

namespace BookingMonolith.Flight.Airports.ValueObjects;

public record Code
{
    public string Value { get; }

    private Code(string value)
    {
        Value = value;
    }

    public static Code Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidCodeException();
        }

        return new Code(value);
    }

    public static implicit operator string(Code code)
    {
        return code.Value;
    }
}
