using SmartCharging.Infrastructure.Exceptions;

namespace Flight.Flights.Exceptions;

public class InvalidArriveDateException : DomainException
{
    public InvalidArriveDateException(DateTime arriveDate)
        : base($"Arrive Date: '{arriveDate}' is invalid.")
    {
    }
}
