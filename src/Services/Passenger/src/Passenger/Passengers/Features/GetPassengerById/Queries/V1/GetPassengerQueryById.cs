using BuildingBlocks.Core.CQRS;
using Passenger.Passengers.Dtos;

namespace Passenger.Passengers.Features.GetPassengerById.Queries.V1;

public record GetPassengerQueryById(long Id) : IQuery<PassengerResponseDto>;
