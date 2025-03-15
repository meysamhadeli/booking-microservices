using BuildingBlocks.Core.Model;

namespace Flight.Airports.Models;

using Features.CreatingAirport.V1;
using ValueObjects;

public record Airport : Aggregate<AirportId>
{
    public Name Name { get; private set; } = default!;
    public Address Address { get; private set; } = default!;
    public Code Code { get; private set; } = default!;

    public static Airport Create(AirportId id, Name name, Address address, Code code, bool isDeleted = false)
    {
        var airport = new Airport
        {
            Id = id,
            Name = name,
            Address = address,
            Code = code
        };

        var @event = new AirportCreatedDomainEvent(
            airport.Id,
            airport.Name,
            airport.Address,
            airport.Code,
            isDeleted);

        airport.AddDomainEvent(@event);

        return airport;
    }
}
