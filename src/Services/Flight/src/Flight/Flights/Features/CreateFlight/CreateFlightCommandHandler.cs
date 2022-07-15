using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.MessageProcessor;
using Flight.Data;
using Flight.Flights.Dtos;
using Flight.Flights.Exceptions;
using Flight.Flights.Features.CreateFlight.Reads;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Flight.Flights.Features.CreateFlight;

public class CreateFlightCommandHandler : ICommandHandler<CreateFlightCommand, FlightResponseDto>
{
    private readonly FlightDbContext _flightDbContext;
    private readonly IPersistMessageProcessor _persistMessageProcessor;
    private readonly IMapper _mapper;

    public CreateFlightCommandHandler(IMapper mapper,
        FlightDbContext flightDbContext,
        IPersistMessageProcessor persistMessageProcessor)
    {
        _mapper = mapper;
        _flightDbContext = flightDbContext;
        _persistMessageProcessor = persistMessageProcessor;
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

        return _mapper.Map<FlightResponseDto>(newFlight.Entity);
    }
}
