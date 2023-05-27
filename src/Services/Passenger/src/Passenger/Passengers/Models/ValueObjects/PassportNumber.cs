namespace Passenger.Passengers.Models.ValueObjects;
using Exceptions;

public record PassportNumber
{
    public string Value { get; }

    public override string ToString()
    {
        return Value;
    }

    private PassportNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidPassportNumberException();
        }
        Value = value;
    }

    public static PassportNumber Of(string value)
    {
        return new PassportNumber(value);
    }

    public static implicit operator string(PassportNumber passportNumber)
    {
        return passportNumber.Value;
    }
}

