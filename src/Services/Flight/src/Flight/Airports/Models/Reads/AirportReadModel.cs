namespace Flight.Airports.Models.Reads;

public class AirportReadModel
{
    public long Id { get; init; }
    public long AirportId { get; init; }
    public string Name { get; init; }
    public string Address { get; init; }
    public string Code { get; init; }
    public bool IsDeleted { get; init; }
}
