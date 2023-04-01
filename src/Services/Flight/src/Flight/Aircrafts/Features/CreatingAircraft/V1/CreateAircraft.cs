namespace Flight.Aircrafts.Features.CreatingAircraft.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Exceptions;
using Models;
using Data;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record CreateAircraft(string Name, string Model, int ManufacturingYear) : ICommand<CreateAircraftResult>, IInternalCommand
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CreateAircraftResult(Guid Id);

internal class CreateAircraftValidator : AbstractValidator<CreateAircraft>
{
    public CreateAircraftValidator()
    {
        RuleFor(x => x.Model).NotEmpty().WithMessage("Model is required");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.ManufacturingYear).NotEmpty().WithMessage("ManufacturingYear is required");
    }
}

internal class CreateAircraftHandler : IRequestHandler<CreateAircraft, CreateAircraftResult>
{
    private readonly FlightDbContext _flightDbContext;

    public CreateAircraftHandler(FlightDbContext flightDbContext)
    {
        _flightDbContext = flightDbContext;
    }

    public async Task<CreateAircraftResult> Handle(CreateAircraft request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var aircraft = await _flightDbContext.Aircraft.SingleOrDefaultAsync(x => x.Model == request.Model, cancellationToken);

        if (aircraft is not null)
        {
            throw new AircraftAlreadyExistException();
        }

        var aircraftEntity = Aircraft.Create(request.Id, request.Name, request.Model, request.ManufacturingYear);

        var newAircraft = (await _flightDbContext.Aircraft.AddAsync(aircraftEntity, cancellationToken))?.Entity;

        return new CreateAircraftResult(newAircraft.Id);
    }
}
