using BuildingBlocks.Domain.Model;
using BuildingBlocks.IdsGenerator;
using Flight.Airports.Events;

namespace Flight.Airports.Models;

public class Airport : Aggregate<long>
{
    public Airport()
    {
    }

    public string Name { get; private set; }
    public string Address { get; private set; }
    public string Code { get; private set; }

    public static Airport Create(long id, string name, string address, string code)
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
            airport.Code);

        airport.AddDomainEvent(@event);

        return airport;
    }
}
