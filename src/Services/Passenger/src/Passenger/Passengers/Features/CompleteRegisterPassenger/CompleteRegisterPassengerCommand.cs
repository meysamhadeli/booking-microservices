using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using BuildingBlocks.IdsGenerator;
using Passenger.Passengers.Dtos;
using Passenger.Passengers.Models;

namespace Passenger.Passengers.Features.CompleteRegisterPassenger;

public record CompleteRegisterPassengerCommand(string PassportNumber, Enums.PassengerType PassengerType, int Age) : ICommand<PassengerResponseDto>, IInternalCommand
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}
