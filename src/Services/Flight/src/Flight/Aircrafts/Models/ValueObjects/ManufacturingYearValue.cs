namespace Flight.Aircrafts.Models.ValueObjects;
using System;

public record ManufacturingYearValue : GenericValueObject<int>
{
    public ManufacturingYearValue(int value) : base(value)
    {
        if (value < 1900 || value > DateTime.Now.Year)
        {
            throw new ArgumentException("Manufacturing year is invalid.");
        }
    }

    public static ManufacturingYearValue Of(int value)
    {
        return new ManufacturingYearValue(value);
    }
}
