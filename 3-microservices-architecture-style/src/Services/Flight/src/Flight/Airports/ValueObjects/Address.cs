namespace Flight.Airports.ValueObjects;

using Exceptions;

public class Address
{
    public string Value { get; }

    private Address(string value)
    {
        Value = value;
    }

    public static Address Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidAddressException();
        }

        return new Address(value);
    }

    public static implicit operator string(Address address)
    {
        return address.Value;
    }
}
