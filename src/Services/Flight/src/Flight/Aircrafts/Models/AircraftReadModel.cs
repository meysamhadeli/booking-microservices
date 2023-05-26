namespace Flight.Aircrafts.Models;

using System;
using Flight.Aircrafts.Models.ValueObjects;

public class AircraftReadModel
{
    public required Guid Id { get; init; }
    public required AircraftId AircraftId { get; init; }
    public required Name Name { get; init; }
    public required Model Model { get; init; }
    public required ManufacturingYear ManufacturingYear { get; init; }
    public required bool IsDeleted { get; init; }
}
