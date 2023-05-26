namespace Flight.Aircrafts.Models.ValueObjects;

using System;

public record ManufacturingYear : GenericValueObject<int>
{
    public ManufacturingYear(int value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value.ToString()))
        {
            throw new ArgumentException("ManufacturingYear cannot be empty or whitespace.");
        }
    }

    public static ManufacturingYear Of(int value)
    {
        return new ManufacturingYear(value);
    }
}
