using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Flight.Flights.Dtos;

namespace Flight.Flights.Features.DeleteFlight.Commands.V1;

public record DeleteFlightCommand(long Id) : ICommand<FlightResponseDto>, IInternalCommand;
