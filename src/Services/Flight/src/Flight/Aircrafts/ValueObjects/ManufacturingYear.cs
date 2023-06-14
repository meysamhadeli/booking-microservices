namespace Flight.Aircrafts.ValueObjects;

using Exceptions;

public record ManufacturingYear
{
    public int Value { get; }
    
    private ManufacturingYear(int value)
    {
        Value = value;
    }

    public static ManufacturingYear Of(int value)
    {
        if (value < 1900)
        {
            throw new InvalidManufacturingYearException();
        }

        return new ManufacturingYear(value);
    }

    public static implicit operator int(ManufacturingYear manufacturingYear)
    {
        return manufacturingYear.Value;
    }
}
