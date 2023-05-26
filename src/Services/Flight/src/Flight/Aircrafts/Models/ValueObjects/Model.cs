namespace Flight.Aircrafts.Models.ValueObjects;
using System;
public record Model : GenericValueObject<string>
{
    public Model(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Model cannot be empty or whitespace.");
        }
    }

    public static Model Of(string value)
    {
        return new Model(value);
    }
}
