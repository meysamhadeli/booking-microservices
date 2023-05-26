namespace Passenger.Passengers.Models.ValueObjects;
using System;

public record Age : GenericValueObject<int>
{
    public Age(int value) : base(value)
    {
        if (value < 0)
        {
            throw new ArgumentException("Age cannot be negative.");
        }
        Value = value;
    }
    public static Age Of(int value)
    {
        return new Age(value);
    }
}
