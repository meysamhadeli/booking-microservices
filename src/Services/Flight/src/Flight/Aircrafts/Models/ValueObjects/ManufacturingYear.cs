namespace Flight.Aircrafts.Models.ValueObjects;

using Flight.Aircrafts.Exceptions;

public record ManufacturingYear
{
    public int Value { get; }
    public ManufacturingYear(int value)
    {
        if (string.IsNullOrWhiteSpace(value.ToString()))
        {
            throw new InvalidManufacturingYearException();
        }

        Value = value;
    }
    public static ManufacturingYear Of(int value)
    {
        return new ManufacturingYear(value);
    }

    public static implicit operator int(ManufacturingYear manufacturingYear)
    {
        return manufacturingYear.Value;
    }
}
