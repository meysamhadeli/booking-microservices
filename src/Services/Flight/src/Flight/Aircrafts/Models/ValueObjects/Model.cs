namespace Flight.Aircrafts.Models.ValueObjects;
using Flight.Aircrafts.Exceptions;

public record Model
{
    public string Value { get; }
    public Model(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidModelException();
        }
        Value = value;
    }
    public static Model Of(string value)
    {
        return new Model(value);
    }

    public static implicit operator string(Model model)
    {
        return model.Value;
    }
}
