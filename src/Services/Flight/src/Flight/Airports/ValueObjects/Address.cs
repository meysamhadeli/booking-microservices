namespace Flight.Airports.ValueObjects;
using Exceptions;

public class Address
{
    public string Value { get; }

    public Address(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidAddressException();
        }
        Value = value;
    }

    public static Address Of(string value)
    {
        return new Address(value);
    }

    public static implicit operator string(Address address)
    {
        return address.Value;
    }
}
