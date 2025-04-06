namespace Flight.Seats.ValueObjects;

using Exceptions;

public record SeatNumber
{
    public string Value { get; }

    private SeatNumber(string value)
    {
        Value = value;
    }

    public static SeatNumber Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidSeatNumberException();
        }

        return new SeatNumber(value);
    }

    public static implicit operator string(SeatNumber seatNumber)
    {
        return seatNumber.Value;
    }
}
