using BuildingBlocks.Core.Event;

namespace Flight.Airports.Features.CreateAirport.Reads;

public class CreateAirportMongoCommand : InternalCommand
{
    public CreateAirportMongoCommand(long id, string name, string address, string code, bool isDeleted)
    {
        Id = id;
        Name = name;
        Address = address;
        Code = code;
        IsDeleted = isDeleted;
    }

    public string Name { get; }
    public string Address { get; }
    public string Code { get; }
    public bool IsDeleted { get; }
}
