namespace Flight.Aircrafts.Dtos;

public record AircraftResponseDto
{
    public long Id { get; set; }
    public string Name { get; init; }
    public string Model { get; init; }
    public int ManufacturingYear { get; init; }
}
