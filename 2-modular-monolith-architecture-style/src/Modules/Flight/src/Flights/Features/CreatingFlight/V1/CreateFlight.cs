namespace Flight.Flights.Features.CreatingFlight.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Aircrafts.ValueObjects;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Web;
using Data;
using Duende.IdentityServer.EntityFramework.Entities;
using Exceptions;
using Flight.Airports.ValueObjects;
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

public record CreateFlight(string FlightNumber, Guid AircraftId, Guid DepartureAirportId,
    DateTime DepartureDate, DateTime ArriveDate, Guid ArriveAirportId,
    decimal DurationMinutes, DateTime FlightDate, Enums.FlightStatus Status,
    decimal Price) : ICommand<CreateFlightResult>, IInternalCommand
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CreateFlightResult(Guid Id);

public record FlightCreatedDomainEvent(Guid Id, string FlightNumber, Guid AircraftId, DateTime DepartureDate,
    Guid DepartureAirportId, DateTime ArriveDate, Guid ArriveAirportId, decimal DurationMinutes,
    DateTime FlightDate, Enums.FlightStatus Status, decimal Price, bool IsDeleted) : IDomainEvent;

public record CreateFlightRequestDto(string FlightNumber, Guid AircraftId, Guid DepartureAirportId,
    DateTime DepartureDate, DateTime ArriveDate, Guid ArriveAirportId,
    decimal DurationMinutes, DateTime FlightDate, Enums.FlightStatus Status, decimal Price);

public record CreateFlightResponseDto(Guid Id);

public class CreateFlightEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/flight", async (CreateFlightRequestDto request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
            {
                var command = mapper.Map<CreateFlight>(request);

                var result = await mediator.Send(command, cancellationToken);

                var response = result.Adapt<CreateFlightResponseDto>();

                return Results.CreatedAtRoute("GetFlightById", new { id = result.Id }, response);
            })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("CreateFlight")
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<CreateFlightResponseDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Flight")
            .WithDescription("Create Flight")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class CreateFlightValidator : AbstractValidator<CreateFlight>
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

        var flightEntity = Models.Flight.Create(FlightId.Of(request.Id), FlightNumber.Of(request.FlightNumber), AircraftId.Of(request.AircraftId),
            AirportId.Of(request.DepartureAirportId), DepartureDate.Of(request.DepartureDate),
            ArriveDate.Of(request.ArriveDate), AirportId.Of(request.ArriveAirportId), DurationMinutes.Of(request.DurationMinutes), FlightDate.Of(request.FlightDate), request.Status,
            Price.Of(request.Price));

        var newFlight = (await _flightDbContext.Flights.AddAsync(flightEntity, cancellationToken)).Entity;

        return new CreateFlightResult(newFlight.Id);
    }
}
