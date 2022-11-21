using BuildingBlocks.Core.Event;

namespace Flight.Airports.Features.CreateAirport.Commands.V1.Reads;

public record CreateAirportMongoCommand(long Id, string Name, string Address, string Code, bool IsDeleted) : InternalCommand;
