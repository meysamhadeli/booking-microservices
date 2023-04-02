namespace Flight.Airports.Models;

using System;

public class AirportReadModel
{
    public required Guid Id { get; init; }
    public required Guid AirportId { get; init; }
    public required string Name { get; init; }
    public string Address { get; init; }
    public required string Code { get; init; }
    public required bool IsDeleted { get; init; }
}
