using BuildingBlocks.Core.Event;

namespace Flight.Aircrafts.Features.CreateAircraft.Commands.V1.Reads;

public class CreateAircraftMongoCommand : InternalCommand
{
    public CreateAircraftMongoCommand(long id, string name, string model, int manufacturingYear, bool isDeleted)
    {
        Id = id;
        Name = name;
        Model = model;
        ManufacturingYear = manufacturingYear;
        IsDeleted = isDeleted;
    }


    public long Id { get; }
    public string Name { get; }
    public string Model { get; }
    public int ManufacturingYear { get; }
    public bool IsDeleted { get; }
}
