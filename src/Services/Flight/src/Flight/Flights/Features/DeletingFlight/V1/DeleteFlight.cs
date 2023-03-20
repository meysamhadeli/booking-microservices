namespace Flight.Flights.Features.DeletingFlight.V1;

using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Data;
using Dtos;
using Exceptions;
using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

public record DeleteFlight(long Id) : ICommand<FlightDto>, IInternalCommand;

internal class DeleteFlightValidator : AbstractValidator<DeleteFlight>
{
    public DeleteFlightValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

internal class DeleteFlightHandler : ICommandHandler<DeleteFlight, FlightDto>
{
    private readonly FlightDbContext _flightDbContext;
    private readonly IMapper _mapper;

    public DeleteFlightHandler(IMapper mapper, FlightDbContext flightDbContext)
    {
        _mapper = mapper;
        _flightDbContext = flightDbContext;
    }

    public async Task<FlightDto> Handle(DeleteFlight request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var flight = await _flightDbContext.Flights.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (flight is null)
        {
            throw new FlightNotFountException();
        }

        var deleteFlight = _flightDbContext.Flights.Remove(flight).Entity;

        flight.Delete(deleteFlight.Id, deleteFlight.FlightNumber, deleteFlight.AircraftId, deleteFlight.DepartureAirportId,
            deleteFlight.DepartureDate, deleteFlight.ArriveDate, deleteFlight.ArriveAirportId, deleteFlight.DurationMinutes,
            deleteFlight.FlightDate, deleteFlight.Status, deleteFlight.Price);

        return _mapper.Map<FlightDto>(deleteFlight);
    }
}
