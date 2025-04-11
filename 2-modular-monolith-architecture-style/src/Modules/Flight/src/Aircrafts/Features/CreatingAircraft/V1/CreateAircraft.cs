namespace Flight.Aircrafts.Features.CreatingAircraft.V1;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Web;
using Data;
using Duende.IdentityServer.EntityFramework.Entities;
using Exceptions;
using Flight.Aircrafts.ValueObjects;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Models;

public record CreateAircraft(string Name, string Model, int ManufacturingYear) : ICommand<CreateAircraftResult>,
    IInternalCommand
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CreateAircraftResult(AircraftId Id);

public record AircraftCreatedDomainEvent
    (Guid Id, string Name, string Model, int ManufacturingYear, bool IsDeleted) : IDomainEvent;

public record CreateAircraftRequestDto(string Name, string Model, int ManufacturingYear);

public record CreateAircraftResponseDto(Guid Id);

public class CreateAircraftEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/flight/aircraft", async (CreateAircraftRequestDto request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
            {
                var command = mapper.Map<CreateAircraft>(request);

                var result = await mediator.Send(command, cancellationToken);

                var response = result.Adapt<CreateAircraftResponseDto>();

                return Results.Ok(response);
            })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("CreateAircraft")
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<CreateAircraftResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Aircraft")
            .WithDescription("Create Aircraft")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class CreateAircraftValidator : AbstractValidator<CreateAircraft>
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

        var aircraft = await _flightDbContext.Aircraft.SingleOrDefaultAsync(
            a => a.Model.Value == request.Model, cancellationToken);

        if (aircraft is not null)
        {
            throw new AircraftAlreadyExistException();
        }

        var aircraftEntity = Aircraft.Create(AircraftId.Of(request.Id), Name.Of(request.Name), Model.Of(request.Model), ManufacturingYear.Of(request.ManufacturingYear));

        var newAircraft = (await _flightDbContext.Aircraft.AddAsync(aircraftEntity, cancellationToken)).Entity;

        return new CreateAircraftResult(newAircraft.Id);
    }
}
