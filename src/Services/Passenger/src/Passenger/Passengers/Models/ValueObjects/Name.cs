namespace Passenger.Passengers.Models.ValueObjects;
using System;

public record Name : GenericValueObject<string>
{
    public Name(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Name cannot be empty or whitespace.");
        }
        Value = value;
    }
    public static Name Of(string value)
    {
        return new Name(value);
    }
}
