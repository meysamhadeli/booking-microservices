namespace Flight.Seats.Features.ReservingSeat.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using Flight.Data;
using Flight.Seats.Exceptions;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

public record ReserveSeat(Guid FlightId, string SeatNumber) : ICommand<ReserveSeatResult>, IInternalCommand;

public record ReserveSeatResult(Guid Id);

public record SeatReservedDomainEvent(Guid Id, string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class,
    Guid FlightId, bool IsDeleted) : IDomainEvent;

public record ReserveSeatRequestDto(Guid FlightId, string SeatNumber);

public record ReserveSeatResponseDto(Guid Id);

public class ReserveSeatEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/flight/reserve-seat", ReserveSeat)
            .RequireAuthorization(nameof(ApiScope))
            .WithName("ReserveSeat")
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<ReserveSeatResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Reserve Seat")
            .WithDescription("Reserve Seat")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> ReserveSeat(ReserveSeatRequestDto request, IMediator mediator, IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<ReserveSeat>(request);

        var result = await mediator.Send(command, cancellationToken);

        var response = result.Adapt<ReserveSeatResponseDto>();

        return Results.Ok(response);
    }
}

public class ReserveSeatValidator : AbstractValidator<ReserveSeat>
{
    public ReserveSeatValidator()
    {
        RuleFor(x => x.FlightId).NotEmpty().WithMessage("FlightId must not be empty");
        RuleFor(x => x.SeatNumber).NotEmpty().WithMessage("SeatNumber must not be empty");
    }
}

internal class ReserveSeatCommandHandler : IRequestHandler<ReserveSeat, ReserveSeatResult>
{
    private readonly FlightDbContext _flightDbContext;

    public ReserveSeatCommandHandler(FlightDbContext flightDbContext)
    {
        _flightDbContext = flightDbContext;
    }

    public async Task<ReserveSeatResult> Handle(ReserveSeat command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var seat = await _flightDbContext.Seats.SingleOrDefaultAsync(
            x => x.SeatNumber.Value == command.SeatNumber &&
                 x.FlightId == command.FlightId, cancellationToken);

        if (seat is null)
        {
            throw new SeatNumberIncorrectException();
        }

        seat.ReserveSeat();

        var updatedSeat = _flightDbContext.Seats.Update(seat).Entity;

        return new ReserveSeatResult(updatedSeat.Id);
    }
}
