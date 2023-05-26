namespace Passenger.Passengers.Models.ValueObjects;
using System;


//public record PassportNumber : GenericValueObject<string>
//{

//    public PassportNumber(string value) : base(value)
//    {
//        if (string.IsNullOrWhiteSpace(value))
//        {
//            throw new ArgumentException("Passport number cannot be empty or whitespace.");
//        }
//        Value = value;
//    }
//    public static PassportNumber Of(string value)
//    {
//        return new PassportNumber(value);
//    }
//}
//public record PassportNumber
//{
//    public string Value { get; }

//    public PassportNumber(string value)
//    {
//        if (string.IsNullOrWhiteSpace(value))
//        {
//            throw new ArgumentException("Passport number cannot be empty or whitespace.");
//        }
//        Value = value;
//    }

//    public static PassportNumber Of(string value)
//    {
//        return new PassportNumber(value);
//    }

//    public static implicit operator string(PassportNumber aircraftId)
//    {
//        return aircraftId.Value;
//    }
//}
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
            throw new ArgumentException("Passport number cannot be empty or whitespace.");
        }
        Value = value;
    }

    public static PassportNumber Of(string value)
    {
        return new PassportNumber(value);
    }
    public static implicit operator string(PassportNumber passportNumber) => passportNumber.Value;
}
