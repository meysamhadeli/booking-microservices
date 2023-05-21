namespace Passenger.Passengers.Models.ValueObjects;
using System;

public record NameValue : GenericValueObject<string>
{
    public NameValue(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Name cannot be empty or whitespace.");
        }
        Value = value;
    }
    public static NameValue Of(string value)
    {
        return new NameValue(value);
    }
}
