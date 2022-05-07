using BuildingBlocks.IdsGenerator;
using Flight.Aircrafts.Dtos;
using MediatR;

namespace Flight.Aircrafts.Features.CreateAircraft;

public record CreateAircraftCommand(string Name, string Model, int ManufacturingYear) : IRequest<AircraftResponseDto>
{
    public long Id { get; set; } = SnowFlakIdGenerator.NewId();
}
