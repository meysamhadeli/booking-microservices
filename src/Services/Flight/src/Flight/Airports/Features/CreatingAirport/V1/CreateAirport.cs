namespace Flight.Airports.Features.CreatingAirport.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Exceptions;
using Data;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record CreateAirport(string Name, string Address, string Code) : ICommand<CreateAirportResult>, IInternalCommand
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CreateAirportResult(Guid Id);

internal class CreateAirportValidator : AbstractValidator<CreateAirport>
{
    public CreateAirportValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("Code is required");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required");
    }
}


internal class CreateAirportHandler : IRequestHandler<CreateAirport, CreateAirportResult>
{
    private readonly FlightDbContext _flightDbContext;

    public CreateAirportHandler(FlightDbContext flightDbContext)
    {
        _flightDbContext = flightDbContext;
    }

    public async Task<CreateAirportResult> Handle(CreateAirport request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var airport = await _flightDbContext.Airports.SingleOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

        if (airport is not null)
        {
            throw new AirportAlreadyExistException();
        }

        var airportEntity = Models.Airport.Create(request.Id, request.Name, request.Code, request.Address);

        var newAirport = (await _flightDbContext.Airports.AddAsync(airportEntity, cancellationToken))?.Entity;

        return new CreateAirportResult(newAirport.Id);
    }
}
