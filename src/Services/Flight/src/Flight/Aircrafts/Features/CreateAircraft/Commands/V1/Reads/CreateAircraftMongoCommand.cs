using BuildingBlocks.Core.Event;

namespace Flight.Aircrafts.Features.CreateAircraft.Commands.V1.Reads;

public record CreateAircraftMongoCommand(long Id, string Name, string Model, int ManufacturingYear, bool IsDeleted) : InternalCommand;
