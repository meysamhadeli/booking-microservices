namespace Flight.Aircrafts.Models.Reads;

public class AircraftReadModel
{
    public long Id { get; init; }
    public long AircraftId { get; init; }
    public string Name { get; init; }
    public string Model { get; init; }
    public int ManufacturingYear { get; init; }
    public bool IsDeleted { get; init; }
}
