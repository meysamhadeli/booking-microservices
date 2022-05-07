namespace Flight.Aircrafts.Dtos;

public record AircraftResponseDto
{
    public string Name { get; init; }
    public string Model { get; init; }
    public int ManufacturingYear { get; init; }
}
