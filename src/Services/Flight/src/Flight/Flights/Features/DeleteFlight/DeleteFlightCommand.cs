using Flight.Flights.Dtos;
using MediatR;

namespace Flight.Flights.Features.DeleteFlight;

public record DeleteFlightCommand(long Id) : IRequest<FlightResponseDto>;
