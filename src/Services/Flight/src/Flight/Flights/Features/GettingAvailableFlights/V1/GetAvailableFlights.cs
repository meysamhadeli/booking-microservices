namespace Flight.Flights.Features.GettingAvailableFlights.V1;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Caching;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Web;
using Data;
using Dtos;
using Duende.IdentityServer.EntityFramework.Entities;
using Exceptions;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;

public record GetAvailableFlights : IQuery<GetAvailableFlightsResult>, ICacheRequest
{
    public string CacheKey => "GetAvailableFlights";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetAvailableFlightsResult(IEnumerable<FlightDto> FlightDtos);

public record GetAvailableFlightsResponseDto(IEnumerable<FlightDto> FlightDtos);

public class GetAvailableFlightsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/flight/get-available-flights",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetAvailableFlights(), cancellationToken);

                    var response = result.Adapt<GetAvailableFlightsResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetAvailableFlights")
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<GetAvailableFlightsResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Available Flights")
            .WithDescription("Get Available Flights")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetAvailableFlightsHandler : IQueryHandler<GetAvailableFlights, GetAvailableFlightsResult>
{
    private readonly IMapper _mapper;
    private readonly FlightReadDbContext _flightReadDbContext;

    public GetAvailableFlightsHandler(IMapper mapper, FlightReadDbContext flightReadDbContext)
    {
        _mapper = mapper;
        _flightReadDbContext = flightReadDbContext;
    }

    public async Task<GetAvailableFlightsResult> Handle(GetAvailableFlights request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var flight = (await _flightReadDbContext.Flight.AsQueryable().ToListAsync(cancellationToken))
            .Where(x => !x.IsDeleted);

        if (!flight.Any())
        {
            throw new FlightNotFountException();
        }

        var flightDtos = _mapper.Map<IEnumerable<FlightDto>>(flight);

        return new GetAvailableFlightsResult(flightDtos);
    }
}
