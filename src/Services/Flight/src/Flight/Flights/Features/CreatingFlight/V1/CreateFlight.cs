namespace Flight.Flights.Features.CreatingFlight.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Data;
using Exceptions;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;

public record CreateFlight(string FlightNumber, Guid AircraftId, Guid DepartureAirportId,
    DateTime DepartureDate, DateTime ArriveDate, Guid ArriveAirportId,
    decimal DurationMinutes, DateTime FlightDate, Enums.FlightStatus Status,
    decimal Price) : ICommand<CreateFlightResult>, IInternalCommand
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CreateFlightResult(Guid Id);

internal class CreateFlightValidator : AbstractValidator<CreateFlight>
{
    public CreateFlightValidator()
    {
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Status).Must(p => (p.GetType().IsEnum &&
                                          p == Enums.FlightStatus.Flying) ||
                                         p == Enums.FlightStatus.Canceled ||
                                         p == Enums.FlightStatus.Delay ||
                                         p == Enums.FlightStatus.Completed)
            .WithMessage("Status must be Flying, Delay, Canceled or Completed");

        RuleFor(x => x.AircraftId).NotEmpty().WithMessage("AircraftId must be not empty");
        RuleFor(x => x.DepartureAirportId).NotEmpty().WithMessage("DepartureAirportId must be not empty");
        RuleFor(x => x.ArriveAirportId).NotEmpty().WithMessage("ArriveAirportId must be not empty");
        RuleFor(x => x.DurationMinutes).GreaterThan(0).WithMessage("DurationMinutes must be greater than 0");
        RuleFor(x => x.FlightDate).NotEmpty().WithMessage("FlightDate must be not empty");
    }
}

internal class CreateFlightHandler : ICommandHandler<CreateFlight, CreateFlightResult>
{
    private readonly FlightDbContext _flightDbContext;

    public CreateFlightHandler(FlightDbContext flightDbContext)
    {
        _flightDbContext = flightDbContext;
    }

    public async Task<CreateFlightResult> Handle(CreateFlight request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var flight = await _flightDbContext.Flights.SingleOrDefaultAsync(x => x.Id == request.Id,
            cancellationToken);

        if (flight is not null)
        {
            throw new FlightAlreadyExistException();
        }

        var flightEntity = Models.Flight.Create(request.Id, request.FlightNumber, request.AircraftId,
            request.DepartureAirportId, request.DepartureDate,
            request.ArriveDate, request.ArriveAirportId, request.DurationMinutes, request.FlightDate, request.Status,
            request.Price);

        var newFlight = (await _flightDbContext.Flights.AddAsync(flightEntity, cancellationToken))?.Entity;

        return new CreateFlightResult(newFlight.Id);
    }
}
