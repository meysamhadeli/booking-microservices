namespace Flight.Flights.Features.GettingFlightById.V1;

using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Dtos;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public class GetFlightByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/flight/{{id}}", GetById)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("GetFlightById")
            .WithMetadata(new SwaggerOperationAttribute("Get Flight By Id", "Get Flight By Id"))
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<FlightDto>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status200OK,
                    "GetFlightById",
                    typeof(FlightDto)))
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

    private async Task<IResult> GetById(long id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetFlightById(id), cancellationToken);

        return Results.Ok(result);
    }
}
