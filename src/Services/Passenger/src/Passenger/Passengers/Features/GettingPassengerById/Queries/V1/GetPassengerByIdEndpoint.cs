namespace Passenger.Passengers.Features.GettingPassengerById.Queries.V1;

using BuildingBlocks.Web;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Passenger.Passengers.Dtos;
using Swashbuckle.AspNetCore.Annotations;

public class GetPassengerByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/passenger/{{id}}", GetById)
            .RequireAuthorization()
            .WithTags("Passenger")
            .WithName("GetPassengerById")
            .WithMetadata(new SwaggerOperationAttribute("Get Passenger By Id", "Get Passenger By Id"))
            .WithApiVersionSet(builder.NewApiVersionSet("Passenger").Build())
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status200OK,
                    "GetPassengerById",
                    typeof(PassengerDto)))
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
        var result = await mediator.Send(new GetPassengerById(id), cancellationToken);

        return Results.Ok(result);
    }
}
