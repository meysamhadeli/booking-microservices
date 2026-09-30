using System.Net;
using Woo.Core.Exception;

namespace Passenger.Exceptions;

public class PassengerNotExist : AppException
{
    public PassengerNotExist() : base("Please register before!", HttpStatusCode.NotFound)
    {
    }
}