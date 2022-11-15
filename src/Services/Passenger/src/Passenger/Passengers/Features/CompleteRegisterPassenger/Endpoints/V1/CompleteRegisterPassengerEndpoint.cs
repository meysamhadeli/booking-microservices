using BuildingBlocks.Web;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Passenger.Passengers.Dtos;
using Passenger.Passengers.Features.CompleteRegisterPassenger.Commands.V1;
using Swashbuckle.AspNetCore.Annotations;

namespace Passenger.Passengers.Features.CompleteRegisterPassenger.Endpoints.V1;

public class CompleteRegisterPassengerEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{EndpointConfig.BaseApiPath}/passenger/complete-registration", CompleteRegisterPassenger)
            .RequireAuthorization()
            .WithTags("Passenger")
            .WithName("Complete Register Passenger")
            .WithMetadata(new SwaggerOperationAttribute("Complete Register Passenger", "Complete Register Passenger"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Passenger").Build())
            .Produces<PassengerResponseDto>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return endpoints;
    }

    private async Task<IResult> CompleteRegisterPassenger(CompleteRegisterPassengerCommand command, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}
