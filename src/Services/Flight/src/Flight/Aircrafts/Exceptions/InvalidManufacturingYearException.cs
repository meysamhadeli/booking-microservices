namespace Flight.Aircrafts.Exceptions;
using BuildingBlocks.Exception;


public class InvalidManufacturingYearException : BadRequestException
{
    public InvalidManufacturingYearException() : base("ManufacturingYear cannot be empty or whitespace.")
    {
    }
}
