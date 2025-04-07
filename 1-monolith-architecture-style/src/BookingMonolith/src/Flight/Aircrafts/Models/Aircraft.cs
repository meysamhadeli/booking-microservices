using BookingMonolith.Flight.Aircrafts.Features.CreatingAircraft.V1;
using BookingMonolith.Flight.Aircrafts.ValueObjects;
using BuildingBlocks.Core.Model;

namespace BookingMonolith.Flight.Aircrafts.Models;

public record Aircraft : Aggregate<AircraftId>
{
    public Name Name { get; private set; } = default!;
    public Model Model { get; private set; } = default!;
    public ManufacturingYear ManufacturingYear { get; private set; } = default!;

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
