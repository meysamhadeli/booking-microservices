using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Flights.Dtos;
using Flight.Flights.Features.UpdateFlight.Commands.V1;
using Flight.Flights.Features.UpdateFlight.Dtos;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace Flight.Flights.Features.UpdateFlight.Endpoints.V1;

public class UpdateFlightEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut($"{EndpointConfig.BaseApiPath}/flight", UpdateFlight)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("UpdateFlight")
            .WithMetadata(new SwaggerOperationAttribute("Update Flight", "Update Flight"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Flight").Build())
            .Produces<FlightResponseDto>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return endpoints;
    }

    private async Task<IResult> UpdateFlight(UpdateFlightRequestDto request, IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        var command = mapper.Map<UpdateFlightCommand>(request);

        var result = await mediator.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
