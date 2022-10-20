using BuildingBlocks.Exception;

namespace Flight.Airports.Features.CreateAirport.Exceptions;

public class AirportAlreadyExistException : ConflictException
{
    public AirportAlreadyExistException(int? code = default) : base("Airport already exist!", code)
    {
    }
}
