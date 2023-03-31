namespace Flight.Aircrafts.Models;

using System;

public class AircraftReadModel
{
    public Guid Id { get; init; }
    public Guid AircraftId { get; init; }
    public string Name { get; init; }
    public string Model { get; init; }
    public int ManufacturingYear { get; init; }
    public bool IsDeleted { get; init; }
}
