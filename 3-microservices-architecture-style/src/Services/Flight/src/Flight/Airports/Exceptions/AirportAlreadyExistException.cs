namespace Flight.Airports.Exceptions;

using BuildingBlocks.Exception;

public class AirportAlreadyExistException : ConflictException
{
    public AirportAlreadyExistException(int? code = default) : base("Airport already exist!", code)
    {
    }
}
