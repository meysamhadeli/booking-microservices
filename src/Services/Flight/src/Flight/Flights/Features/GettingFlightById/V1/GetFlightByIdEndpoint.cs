namespace Flight.Flights.Features.GettingFlightById.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Dtos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

public record GetFlightByIdResponseDto(FlightDto FlightDto);

public class GetFlightByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/flight/{{id}}", GetById)
            .RequireAuthorization()
            .WithName("GetFlightById")
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<GetFlightByIdResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation => new OpenApiOperation(operation) { Summary = "Get Flight By Id", Description = "Get Flight By Id" })
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> GetById(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetFlightById(id), cancellationToken);

        var response = new GetFlightByIdResponseDto(result?.FlightDto);

        return Results.Ok(response);
    }
}
