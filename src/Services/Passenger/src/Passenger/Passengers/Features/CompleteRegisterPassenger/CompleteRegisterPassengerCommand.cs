using BuildingBlocks.Core.CQRS;
using BuildingBlocks.IdsGenerator;
using Passenger.Passengers.Dtos;
using Passenger.Passengers.Models;

namespace Passenger.Passengers.Features.CompleteRegisterPassenger;

public record CompleteRegisterPassengerCommand(string PassportNumber, PassengerType PassengerType, int Age) : ICommand<PassengerResponseDto>
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}
