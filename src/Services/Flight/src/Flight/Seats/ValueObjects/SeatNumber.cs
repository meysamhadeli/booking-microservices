namespace Flight.Seats.ValueObjects;

using Exceptions;

public record SeatNumber
{
    public string Value { get; }

    public SeatNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidSeatNumberException();
        }
        Value = value;
    }
    public static SeatNumber Of(string value)
    {
        return new SeatNumber(value);
    }

    public static implicit operator string(SeatNumber seatNumber)
    {
        return seatNumber.Value;
    }
}
