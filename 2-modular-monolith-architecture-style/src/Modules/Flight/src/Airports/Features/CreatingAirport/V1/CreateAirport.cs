namespace Flight.Airports.Features.CreatingAirport.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Web;
using Data;
using Duende.IdentityServer.EntityFramework.Entities;
using Exceptions;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ValueObjects;

public record CreateAirport(string Name, string Address, string Code) : ICommand<CreateAirportResult>, IInternalCommand
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CreateAirportResult(Guid Id);

public record AirportCreatedDomainEvent
    (Guid Id, string Name, string Address, string Code, bool IsDeleted) : IDomainEvent;

public record CreateAirportRequestDto(string Name, string Address, string Code);

public record CreateAirportResponseDto(Guid Id);

public class CreateAirportEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/flight/airport", async (CreateAirportRequestDto request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
            {
                var command = mapper.Map<CreateAirport>(request);

                var result = await mediator.Send(command, cancellationToken);

                var response = result.Adapt<CreateAirportResponseDto>();

                return Results.Ok(response);
            })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("CreateAirport")
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<CreateAirportResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Airport")
            .WithDescription("Create Airport")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class CreateAirportValidator : AbstractValidator<CreateAirport>
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

        var airport =
            await _flightDbContext.Airports.SingleOrDefaultAsync(x => x.Code.Value == request.Code, cancellationToken);

        if (airport is not null)
        {
            throw new AirportAlreadyExistException();
        }

        var airportEntity = Models.Airport.Create(AirportId.Of(request.Id), Name.Of(request.Name), Address.Of(request.Address), Code.Of(request.Code));

        var newAirport = (await _flightDbContext.Airports.AddAsync(airportEntity, cancellationToken)).Entity;

        return new CreateAirportResult(newAirport.Id);
    }
}
