using BuildingBlocks.Web;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Passenger.Passengers.Dtos;
using Passenger.Passengers.Features.CompleteRegisterPassenger.Commands.V1;
using Passenger.Passengers.Features.CompleteRegisterPassenger.Dtos.V1;
using Swashbuckle.AspNetCore.Annotations;

namespace Passenger.Passengers.Features.CompleteRegisterPassenger.Endpoints.V1;

public class CompleteRegisterPassengerEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{EndpointConfig.BaseApiPath}/passenger/complete-registration", CompleteRegisterPassenger)
            .RequireAuthorization()
            .WithTags("Passenger")
            .WithName("CompleteRegisterPassenger")
            .WithMetadata(new SwaggerOperationAttribute("Complete Register Passenger", "Complete Register Passenger"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Passenger").Build())
            .Produces<PassengerResponseDto>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return endpoints;
    }

    private async Task<IResult> CompleteRegisterPassenger(CompleteRegisterPassengerRequestDto request, IMapper mapper,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var command = mapper.Map<CompleteRegisterPassengerCommand>(request);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}
