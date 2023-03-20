namespace Flight.Flights.Features.GettingAvailableFlights.V1;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Flights.Dtos;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public class GetAvailableFlightsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/flight/get-available-flights", GetAvailableFlights)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("GetAvailableFlights")
            .WithMetadata(new SwaggerOperationAttribute("Get Available Flights", "Get Available Flights"))
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status200OK,
                    "GetAvailableFlights",
                    typeof(IEnumerable<FlightDto>)))
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

        return builder;
    }

    private async Task<IResult> GetAvailableFlights(IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAvailableFlights(), cancellationToken);

        return Results.Ok(result);
    }
}
