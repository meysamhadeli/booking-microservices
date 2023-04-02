namespace Flight.Aircrafts.Models;

using System;

public class AircraftReadModel
{
    public required Guid Id { get; init; }
    public required Guid AircraftId { get; init; }
    public required string Name { get; init; }
    public required string Model { get; init; }
    public required int ManufacturingYear { get; init; }
    public required bool IsDeleted { get; init; }
}
