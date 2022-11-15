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
public class GetPassengerByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{EndpointConfig.BaseApiPath}/passenger/{{id}}", GetById)
            .RequireAuthorization()
            .WithTags("Passenger")
            .WithName("Get Passenger By Id")
            .WithMetadata(new SwaggerOperationAttribute("Get Passenger By Id", "Get Passenger By Id"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Passenger").Build())
            .Produces<PassengerResponseDto>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return endpoints;
    }

    private async Task<IResult> GetById(long id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPassengerQueryById(id), cancellationToken);

        return Results.Ok(result);
    }
}
