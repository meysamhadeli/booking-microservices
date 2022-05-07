using MediatR;
using Passenger.Passengers.Dtos;

namespace Passenger.Passengers.Features.GetPassengerById;

public record GetPassengerQueryById(long Id) : IRequest<PassengerResponseDto>;