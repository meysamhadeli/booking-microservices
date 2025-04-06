using MongoDB.Driver.Linq;

namespace Flight.Seats.Features.GettingAvailableSeats.V1;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Web;
using Data;
using Dtos;
using Duende.IdentityServer.EntityFramework.Entities;
using Exceptions;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;

public record GetAvailableSeats(Guid FlightId) : IQuery<GetAvailableSeatsResult>;

public record GetAvailableSeatsResult(IEnumerable<SeatDto> SeatDtos);

public record GetAvailableSeatsResponseDto(IEnumerable<SeatDto> SeatDtos);

public class GetAvailableSeatsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/flight/get-available-seats/{{id}}", GetAvailableSeats)
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetAvailableSeats")
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<GetAvailableSeatsResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Available Seats")
            .WithDescription("Get Available Seats")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> GetAvailableSeats(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAvailableSeats(id), cancellationToken);

        var response = result.Adapt<GetAvailableSeatsResponseDto>();

        return Results.Ok(response);
    }
}

public class GetAvailableSeatsValidator : AbstractValidator<GetAvailableSeats>
{
    public GetAvailableSeatsValidator()
    {
        RuleFor(x => x.FlightId).NotNull().WithMessage("FlightId is required!");
    }
}

internal class GetAvailableSeatsQueryHandler : IRequestHandler<GetAvailableSeats, GetAvailableSeatsResult>
{
    private readonly IMapper _mapper;
    private readonly FlightReadDbContext _flightReadDbContext;

    public GetAvailableSeatsQueryHandler(IMapper mapper, FlightReadDbContext flightReadDbContext)
    {
        _mapper = mapper;
        _flightReadDbContext = flightReadDbContext;
    }


    public async Task<GetAvailableSeatsResult> Handle(GetAvailableSeats query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var seats = (await _flightReadDbContext.Seat.AsQueryable().ToListAsync(cancellationToken))
            .Where(x => x.FlightId == query.FlightId && !x.IsDeleted);

        if (!seats.Any())
        {
            throw new AllSeatsFullException();
        }

        var seatDtos = _mapper.Map<IEnumerable<SeatDto>>(seats);

        return new GetAvailableSeatsResult(seatDtos);
    }
}
