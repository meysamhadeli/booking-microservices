using System.Net;
using Woo.Core.Exception;

namespace Flight.Aircrafts.Exceptions;

public class AircraftAlreadyExistException : AppException
{
    public AircraftAlreadyExistException() : base("Aircraft already exist!", HttpStatusCode.Conflict)
    {
    }
}