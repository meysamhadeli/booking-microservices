namespace Flight.Aircrafts.Exceptions;

using System.Net;
using BuildingBlocks.Exception;

public class AircraftAlreadyExistException : AppException
{
    public AircraftAlreadyExistException() : base("Aircraft already exist!", HttpStatusCode.Conflict)
    {
    }
}
