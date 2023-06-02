namespace Flight.Aircrafts.Exceptions;
using BuildingBlocks.Exception;


public class InvalidManufacturingYearException : BadRequestException
{
    public InvalidManufacturingYearException() : base("ManufacturingYear must be greater than 1900")
    {
    }
}
