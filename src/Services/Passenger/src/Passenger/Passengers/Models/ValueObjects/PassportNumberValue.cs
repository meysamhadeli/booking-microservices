namespace Passenger.Passengers.Models.ValueObjects;
using System;


public record PassportNumberValue : GenericValueObject<string>
{
    public PassportNumberValue(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Passport number cannot be empty or whitespace.");
        }
        Value = value;
    }
    public static PassportNumberValue Of(string value)
    {
        return new PassportNumberValue(value);
    }
}
