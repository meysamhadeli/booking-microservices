namespace Passenger.Passengers.Models.ValueObjects;
using global::Passenger.Passengers.Exceptions;

public record Age
{
    public int Value { get; }
    public Age(int value)
    {
        if (value <= 0 || value == null)
        {
            throw new InvalidAgeException();
        }

        Value = value;
    }
    public static Age Of(int value)
    {
        return new Age(value);
    }

    public static implicit operator int(Age age)
    {
        return age.Value;
    }
}
