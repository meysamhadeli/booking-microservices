using BuildingBlocks.Core.Model;

namespace Flight.Aircrafts.Models;

using System;
using Features.CreatingAircraft.V1;
using ValueObjects;

public record Aircraft : Aggregate<Guid>
{
    public NameValue Name { get; private set; }
    public ModelValue Model { get; private set; }
    public ManufacturingYearValue ManufacturingYear { get; private set; }

    public static Aircraft Create(Guid id, NameValue name, ModelValue model, ManufacturingYearValue manufacturingYear, bool isDeleted = false)
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
