using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Flights.Dtos;
using Flight.Flights.Features.GetAvailableFlights.Queries.V1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace Flight.Flights.Features.GetAvailableFlights.Endpoints.V1;

using Hellang.Middleware.ProblemDetails;

public class GetAvailableFlightsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{EndpointConfig.BaseApiPath}/flight/get-available-flights", GetAvailableFlights)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("GetAvailableFlights")
            .WithMetadata(new SwaggerOperationAttribute("Get Available Flights", "Get Available Flights"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Flight").Build())
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status200OK,
                    "GetAvailableFlights",
                    typeof(IEnumerable<FlightResponseDto>)))
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status400BadRequest,
                    "BadRequest",
                    typeof(StatusCodeProblemDetails)))
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status401Unauthorized,
                    "UnAuthorized",
                    typeof(StatusCodeProblemDetails)))
            .HasApiVersion(1.0);

        return endpoints;
    }

    private async Task<IResult> GetAvailableFlights(IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAvailableFlightsQuery(), cancellationToken);

        return Results.Ok(result);
    }
}
