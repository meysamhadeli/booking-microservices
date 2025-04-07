using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Aircrafts.Exceptions;

public class InvalidManufacturingYearException : BadRequestException
{
    public InvalidManufacturingYearException() : base("ManufacturingYear must be greater than 1900")
    {
    }
}
