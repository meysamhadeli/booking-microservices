using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using BuildingBlocks.IdsGenerator;
using Flight.Airports.Dtos;

namespace Flight.Airports.Features.CreateAirport.Commands.V1;

public record CreateAirportCommand(string Name, string Address, string Code) : ICommand<AirportResponseDto>, IInternalCommand
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}
