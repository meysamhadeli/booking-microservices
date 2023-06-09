namespace Flight.Airports.ValueObjects;
using System.Linq;
using Exceptions;

public class Address
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string ZipCode { get; }
    public string Country { get; }
    public string Value { get; }

    public Address(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidAddressException();
        }
        Value = value;
    }

    public Address(string street, string city, string state, string zipCode, string country)
    {
        Street = street?.Trim();
        City = city?.Trim();
        State = state?.Trim();
        ZipCode = zipCode?.Trim();
        Country = country?.Trim();

        Value = string.Join(", ", new[] { Street, City, State, ZipCode, Country }.Where(s => !string.IsNullOrWhiteSpace(s)));
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
