namespace Flight.Flights.ValueObjects;
using Exceptions;

public record FlightNumber
{
    public string Value { get; }
    public FlightNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidFlightNumberException(value);
        }
        Value = value;
    }
    public static FlightNumber Of(string value)
    {
        return new FlightNumber(value);
    }

    public static implicit operator string(FlightNumber flightNumber)
    {
        return flightNumber.Value;
    }
}
