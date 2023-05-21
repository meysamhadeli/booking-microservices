namespace Passenger.Passengers.Models.ValueObjects;
using System;

public record AgeValue : GenericValueObject<int>
{
    public AgeValue(int value) : base(value)
    {
        if (value < 0)
        {
            throw new ArgumentException("Age cannot be negative.");
        }
        Value = value;
    }
    public static AgeValue Of(int value)
    {
        return new AgeValue(value);
    }
}
