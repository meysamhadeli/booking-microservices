using System.Net;
using BuildingBlocks.Exception;

namespace Flight.Aircrafts.Features.CreateAircraft.Exceptions;

public class AircraftAlreadyExistException : AppException
{
    public AircraftAlreadyExistException() : base("Aircraft already exist!", HttpStatusCode.Conflict)
    {
    }
}
