using BuildingBlocks.Web;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Passenger.Passengers.Dtos;
using Passenger.Passengers.Features.GetPassengerById.Queries.V1;
using Swashbuckle.AspNetCore.Annotations;

namespace Passenger.Passengers.Features.GetPassengerById.Endpoints.V1;

using Hellang.Middleware.ProblemDetails;

public class GetPassengerByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{EndpointConfig.BaseApiPath}/passenger/{{id}}", GetById)
            .RequireAuthorization()
            .WithTags("Passenger")
            .WithName("GetPassengerById")
            .WithMetadata(new SwaggerOperationAttribute("Get Passenger By Id", "Get Passenger By Id"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Passenger").Build())
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status200OK,
                    "GetPassengerById",
                    typeof(PassengerResponseDto)))
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

    private async Task<IResult> GetById(long id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPassengerQueryById(id), cancellationToken);

        return Results.Ok(result);
    }
}
