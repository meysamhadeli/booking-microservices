namespace Flight.Flights.Features.GettingAvailableFlights.V1;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Dtos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

public record GetAvailableFlightsResponseDto(IEnumerable<FlightDto> FlightDtos);

public class GetAvailableFlightsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/flight/get-available-flights", GetAvailableFlights)
            .RequireAuthorization()
            .WithName("GetAvailableFlights")
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<GetAvailableFlightsResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation => new OpenApiOperation(operation) { Summary = "Get Available Flights", Description = "Get Available Flights" })
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> GetAvailableFlights(IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAvailableFlights(), cancellationToken);

        var response = new GetAvailableFlightsResponseDto(result?.FlightDtos);

        return Results.Ok(response);
    }
}
