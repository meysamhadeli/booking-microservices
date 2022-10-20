using System.Net;
using BuildingBlocks.Exception;

namespace Flight.Aircrafts.Features.CreateAircraft.Exceptions;

public class AircraftAlreadyExistException : AppException
{
    public AircraftAlreadyExistException() : base("Flight already exist!", HttpStatusCode.Conflict)
    {
    }
}
