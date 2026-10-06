using Griffin.Core.Exception;

namespace Flight.Flights.Exceptions;

public class InvalidArriveDateException : DomainException
{
    public InvalidArriveDateException(DateTime arriveDate)
        : base($"Arrive Date: '{arriveDate}' is invalid.") { }
}
