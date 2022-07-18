using BuildingBlocks.Core.CQRS;
using BuildingBlocks.IdsGenerator;
using Flight.Airports.Dtos;
using MediatR;

namespace Flight.Airports.Features.CreateAirport;

public record CreateAirportCommand(string Name, string Address, string Code) : ICommand<AirportResponseDto>, IInternalCommand
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}
