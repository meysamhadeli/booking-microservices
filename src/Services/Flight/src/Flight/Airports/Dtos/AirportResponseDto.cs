namespace Flight.Airports.Dtos;
public record AirportResponseDto
{
    public long Id { get; set; }
    public string Name { get; init; }
    public string Address { get; init; }
    public string Code { get; init; }
}
