namespace Flight.Aircrafts.ValueObjects;

using Flight.Aircrafts.Exceptions;

public record Name
{
    public string Value { get; }
    public Name(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidNameException();
        }
        Value = value;
    }
    public static Name Of(string value)
    {
        return new Name(value);
    }

    public static implicit operator string(Name name)
    {
        return name.Value;
    }
}
