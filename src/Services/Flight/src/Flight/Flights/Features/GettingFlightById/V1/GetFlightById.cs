namespace Flight.Flights.Features.GettingFlightById.V1;

using System;
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
using MongoDB.Driver.Linq;

public record GetFlightById(Guid Id) : IQuery<GetFlightByIdResult>;

public record GetFlightByIdResult(FlightDto FlightDto);

public record GetFlightByIdResponseDto(FlightDto FlightDto);

public class GetFlightByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/flight/{{id}}",
                async (Guid id, IMediator mediator, IMapper mapper, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetFlightById(id), cancellationToken);

                    var response = result.Adapt<GetFlightByIdResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetFlightById")
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<GetFlightByIdResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Flight By Id")
            .WithDescription("Get Flight By Id")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class GetFlightByIdValidator : AbstractValidator<GetFlightById>
{
    public GetFlightByIdValidator()
    {
        RuleFor(x => x.Id).NotNull().WithMessage("Id is required!");
    }
}

internal class GetFlightByIdHandler : IQueryHandler<GetFlightById, GetFlightByIdResult>
{
    private readonly IMapper _mapper;
    private readonly FlightReadDbContext _flightReadDbContext;

    public GetFlightByIdHandler(IMapper mapper, FlightReadDbContext flightReadDbContext)
    {
        _mapper = mapper;
        _flightReadDbContext = flightReadDbContext;
    }

    public async Task<GetFlightByIdResult> Handle(GetFlightById request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var flight =
            await _flightReadDbContext.Flight.AsQueryable().SingleOrDefaultAsync(x => x.FlightId == request.Id &&
                !x.IsDeleted, cancellationToken);

        if (flight is null)
        {
            throw new FlightNotFountException();
        }

        var flightDto = _mapper.Map<FlightDto>(flight);

        return new GetFlightByIdResult(flightDto);
    }
}
