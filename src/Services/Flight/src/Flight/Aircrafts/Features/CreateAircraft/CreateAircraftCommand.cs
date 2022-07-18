using BuildingBlocks.Core.CQRS;
using BuildingBlocks.IdsGenerator;
using Flight.Aircrafts.Dtos;
using MediatR;

namespace Flight.Aircrafts.Features.CreateAircraft;

public record CreateAircraftCommand(string Name, string Model, int ManufacturingYear) : ICommand<AircraftResponseDto>, IInternalCommand
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}
