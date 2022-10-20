using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using BuildingBlocks.IdsGenerator;
using Flight.Aircrafts.Dtos;

namespace Flight.Aircrafts.Features.CreateAircraft.Commands.V1;

public record CreateAircraftCommand(string Name, string Model, int ManufacturingYear) : ICommand<AircraftResponseDto>, IInternalCommand
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}
