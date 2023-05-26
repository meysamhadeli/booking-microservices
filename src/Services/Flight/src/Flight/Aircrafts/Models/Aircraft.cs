using BuildingBlocks.Core.Model;

namespace Flight.Aircrafts.Models;

using Features.CreatingAircraft.V1;
using ValueObjects;

public record Aircraft : Aggregate<AircraftId>
{
    public Name Name { get; private set; }
    public Model Model { get; private set; }
    public ManufacturingYear ManufacturingYear { get; private set; }

    public static Aircraft Create(AircraftId id, Name name, Model model, ManufacturingYear manufacturingYear, bool isDeleted = false)
    {
        var aircraft = new Aircraft
        {
            Id = id,
            Name = name,
            Model = model,
            ManufacturingYear = manufacturingYear
        };

        var @event = new AircraftCreatedDomainEvent(
            aircraft.Id,
            aircraft.Name,
            aircraft.Model,
            aircraft.ManufacturingYear,
            isDeleted);

        aircraft.AddDomainEvent(@event);

        return aircraft;
    }
}
