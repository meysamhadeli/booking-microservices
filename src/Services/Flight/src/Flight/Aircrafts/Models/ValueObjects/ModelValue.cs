namespace Flight.Aircrafts.Models.ValueObjects;
using System;
public record ModelValue : GenericValueObject<string>
{
    public ModelValue(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Model cannot be empty or whitespace.");
        }
    }

    public static ModelValue Of(string value)
    {
        return new ModelValue(value);
    }
}
