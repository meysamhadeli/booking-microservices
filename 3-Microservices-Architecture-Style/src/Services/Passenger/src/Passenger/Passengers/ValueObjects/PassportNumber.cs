namespace Passenger.Passengers.ValueObjects;

using Passenger.Passengers.Exceptions;

public record PassportNumber
{
    public string Value { get; }

    public override string ToString()
    {
        return Value;
    }

    private PassportNumber(string value)
    {
        Value = value;
    }

    public static PassportNumber Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidPassportNumberException();
        }

        return new PassportNumber(value);
    }

    public static implicit operator string(PassportNumber passportNumber)
    {
        return passportNumber.Value;
    }
}
