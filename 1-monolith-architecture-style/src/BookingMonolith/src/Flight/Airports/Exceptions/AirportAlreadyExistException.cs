using System.Net;
using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Airports.Exceptions;

public class AirportAlreadyExistException : AppException
{
    public AirportAlreadyExistException(int? code = default) : base("Airport already exist!", HttpStatusCode.Conflict, code)
    {
    }
}
