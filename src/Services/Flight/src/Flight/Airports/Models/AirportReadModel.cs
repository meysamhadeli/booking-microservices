namespace Flight.Airports.Models;

using System;

public class AirportReadModel
{
    public Guid Id { get; init; }
    public Guid AirportId { get; init; }
    public string Name { get; init; }
    public string Address { get; init; }
    public string Code { get; init; }
    public bool IsDeleted { get; init; }
}
