using Griffin.Core.Exception;

namespace Flight.Aircrafts.Exceptions;

public class InvalidManufacturingYearException : DomainException
{
    public InvalidManufacturingYearException()
        : base("ManufacturingYear must be greater than 1900") { }
}
