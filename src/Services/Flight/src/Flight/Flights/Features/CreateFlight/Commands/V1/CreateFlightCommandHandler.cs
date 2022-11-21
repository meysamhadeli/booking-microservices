using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using Flight.Data;
using Flight.Flights.Dtos;
using Flight.Flights.Exceptions;
using Flight.Flights.Features.CreateFlight.Exceptions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Flight.Flights.Features.CreateFlight.Commands.V1;

public class CreateFlightCommandHandler : ICommandHandler<CreateFlightCommand, FlightResponseDto>
{
    private readonly FlightDbContext _flightDbContext;
    private readonly IMapper _mapper;

    public CreateFlightCommandHandler(IMapper mapper,
        FlightDbContext flightDbContext)
    {
        _mapper = mapper;
        _flightDbContext = flightDbContext;
    }

    public async Task<FlightResponseDto> Handle(CreateFlightCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var flight = await _flightDbContext.Flights.SingleOrDefaultAsync(x => x.Id == command.Id,
            cancellationToken);

        if (flight is not null)
            throw new FlightAlreadyExistException();

        var flightEntity = Models.Flight.Create(command.Id, command.FlightNumber, command.AircraftId,
            command.DepartureAirportId, command.DepartureDate,
            command.ArriveDate, command.ArriveAirportId, command.DurationMinutes, command.FlightDate, command.Status,
            command.Price);

        var newFlight = await _flightDbContext.Flights.AddAsync(flightEntity, cancellationToken);

        var f = _mapper.Map<FlightResponseDto>(newFlight.Entity);

        return f;
    }
}
