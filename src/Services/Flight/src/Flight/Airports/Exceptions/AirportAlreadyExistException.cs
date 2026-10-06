using System.Net;
using Griffin.Core.Exception;

namespace Flight.Airports.Exceptions;

public class AirportAlreadyExistException : AppException
{
    public AirportAlreadyExistException(int? code = default) : base("Airport already exist!", HttpStatusCode.Conflict, code)
    {
    }
}