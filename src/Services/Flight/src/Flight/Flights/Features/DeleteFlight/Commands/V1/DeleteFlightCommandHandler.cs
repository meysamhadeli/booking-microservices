using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using Flight.Data;
using Flight.Flights.Dtos;
using Flight.Flights.Exceptions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Flight.Flights.Features.DeleteFlight.Commands.V1;

public class DeleteFlightCommandHandler : ICommandHandler<DeleteFlightCommand, FlightResponseDto>
{
    private readonly FlightDbContext _flightDbContext;
    private readonly IMapper _mapper;

    public DeleteFlightCommandHandler(IMapper mapper, FlightDbContext flightDbContext)
    {
        _mapper = mapper;
        _flightDbContext = flightDbContext;
    }

    public async Task<FlightResponseDto> Handle(DeleteFlightCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var flight = await _flightDbContext.Flights.SingleOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (flight is null)
            throw new FlightNotFountException();


        var deleteFlight = _flightDbContext.Flights.Remove(flight).Entity;

        flight.Delete(deleteFlight.Id, deleteFlight.FlightNumber, deleteFlight.AircraftId, deleteFlight.DepartureAirportId,
            deleteFlight.DepartureDate, deleteFlight.ArriveDate, deleteFlight.ArriveAirportId, deleteFlight.DurationMinutes,
            deleteFlight.FlightDate, deleteFlight.Status, deleteFlight.Price);

        return _mapper.Map<FlightResponseDto>(deleteFlight);
    }
}
