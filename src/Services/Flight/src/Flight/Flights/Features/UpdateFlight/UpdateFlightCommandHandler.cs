using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.EventStoreDB.Repository;
using Flight.Data;
using Flight.Flights.Dtos;
using Flight.Flights.Exceptions;
using Flight.Flights.Models;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flight.Flights.Features.UpdateFlight;

public class UpdateFlightCommandHandler : IRequestHandler<UpdateFlightCommand, FlightResponseDto>
{
    private readonly FlightDbContext _flightDbContext;
    private readonly IMapper _mapper;

    public UpdateFlightCommandHandler(IMapper mapper, FlightDbContext flightDbContext)
    {
        _mapper = mapper;
        _flightDbContext = flightDbContext;
    }

    public async Task<FlightResponseDto> Handle(UpdateFlightCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var flight = await _flightDbContext.Flights.SingleOrDefaultAsync(x => x.FlightNumber == command.FlightNumber && !x.IsDeleted,
            cancellationToken);

        if (flight is null)
            throw new FlightNotFountException();


        flight.Update(command.Id, command.FlightNumber, command.AircraftId, command.DepartureAirportId, command.DepartureDate,
            command.ArriveDate, command.ArriveAirportId, command.DurationMinutes, command.FlightDate, FlightStatus.Completed, command.Price, command.IsDeleted);

        var updateFlight = _flightDbContext.Flights.Update(flight);

        return _mapper.Map<FlightResponseDto>(updateFlight.Entity);
    }
}
