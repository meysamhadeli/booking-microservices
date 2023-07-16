namespace Flight.Flights.Features.DeletingFlight.V1;

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
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using ValueObjects;

public record DeleteFlight(Guid Id) : ICommand<DeleteFlightResult>, IInternalCommand;

public record DeleteFlightResult(Guid Id);

public record FlightDeletedDomainEvent(Guid Id, string FlightNumber, Guid AircraftId, DateTime DepartureDate,
    Guid DepartureAirportId, DateTime ArriveDate, Guid ArriveAirportId, decimal DurationMinutes,
    DateTime FlightDate, Enums.FlightStatus Status, decimal Price, bool IsDeleted) : IDomainEvent;

public class DeleteFlightEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/flight/{{id}}",
                async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    await mediator.Send(new DeleteFlight(id), cancellationToken);

                    return Results.NoContent();
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DeleteFlight")
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Flight")
            .WithDescription("Delete Flight")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class DeleteFlightValidator : AbstractValidator<DeleteFlight>
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

        var deleteFlight = _flightDbContext.Flights.Update(flight).Entity;

        return new DeleteFlightResult(deleteFlight.Id);
    }
}
