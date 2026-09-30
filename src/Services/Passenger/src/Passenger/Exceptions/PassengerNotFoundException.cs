using System.Net;
using Woo.Core.Exception;

namespace Passenger.Exceptions;

public class PassengerNotFoundException : AppException
{
    public PassengerNotFoundException() : base("Passenger not found!", HttpStatusCode.NotFound)
    {
    }
}