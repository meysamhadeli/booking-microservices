namespace Flight.Flights.Features.DeletingFlight.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Data;
using Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

public record DeleteFlight(Guid Id) : ICommand<DeleteFlightResult>, IInternalCommand;

public record DeleteFlightResult(Guid Id);

internal class DeleteFlightValidator : AbstractValidator<DeleteFlight>
{
    public DeleteFlightValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

internal class DeleteFlightHandler : ICommandHandler<DeleteFlight, DeleteFlightResult>
{
    private readonly FlightDbContext _flightDbContext;

    public DeleteFlightHandler(FlightDbContext flightDbContext)
    {
        _flightDbContext = flightDbContext;
    }

    public async Task<DeleteFlightResult> Handle(DeleteFlight request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var flight = await _flightDbContext.Flights.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (flight is null)
        {
            throw new FlightNotFountException();
        }

        flight.Delete(flight.Id, flight.FlightNumber, flight.AircraftId, flight.DepartureAirportId,
            flight.DepartureDate, flight.ArriveDate, flight.ArriveAirportId, flight.DurationMinutes,
            flight.FlightDate, flight.Status, flight.Price);

        var deleteFlight = (_flightDbContext.Flights.Remove(flight))?.Entity;

        return new DeleteFlightResult(deleteFlight.Id);
    }
}
