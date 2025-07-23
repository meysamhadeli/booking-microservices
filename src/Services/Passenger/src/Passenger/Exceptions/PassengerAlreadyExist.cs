using System.Net;
using BuildingBlocks.Exception;

namespace Passenger.Exceptions;

public class PassengerNotExist : AppException
{
    public PassengerNotExist() : base("Please register before!", HttpStatusCode.NotFound)
    {
    }
}