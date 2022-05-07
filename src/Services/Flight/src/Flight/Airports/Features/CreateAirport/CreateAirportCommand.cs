using BuildingBlocks.IdsGenerator;
using Flight.Airports.Dtos;
using MediatR;

namespace Flight.Airports.Features.CreateAirport;

public record CreateAirportCommand(string Name, string Address, string Code) : IRequest<AirportResponseDto>
{
    public long Id { get; set; } = SnowFlakIdGenerator.NewId();
}
