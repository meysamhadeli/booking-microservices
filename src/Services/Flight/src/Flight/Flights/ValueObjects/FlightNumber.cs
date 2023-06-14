namespace Flight.Flights.ValueObjects;

using Exceptions;

public record FlightNumber
{
    public string Value { get; }

    private FlightNumber(string value)
    {
        Value = value;
    }

    public static FlightNumber Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidFlightNumberException(value);
        }

        return new FlightNumber(value);
    }

    public static implicit operator string(FlightNumber flightNumber)
    {
        return flightNumber.Value;
    }
}
