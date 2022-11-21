using BuildingBlocks.Core.Event;

namespace Passenger.Passengers.Features.CompleteRegisterPassenger.Commands.V1.Reads;

public record CompleteRegisterPassengerMongoCommand(long Id, string PassportNumber, string Name,
    Enums.PassengerType PassengerType, int Age, bool IsDeleted) : InternalCommand;
