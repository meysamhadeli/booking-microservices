namespace Flight.Airports.ValueObjects;

using Exceptions;

public record Code
{
    public string Value { get; }
    public Code(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidCodeException();
        }
        Value = value;
    }
    public static Code Of(string value)
    {
        return new Code(value);
    }

    public static implicit operator string(Code code)
    {
        return code.Value;
    }
}
