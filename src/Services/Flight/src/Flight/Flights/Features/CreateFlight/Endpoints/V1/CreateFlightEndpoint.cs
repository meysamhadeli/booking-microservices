using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Flights.Dtos;
using Flight.Flights.Features.CreateFlight.Commands.V1;
using Flight.Flights.Features.CreateFlight.Dtos.V1;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace Flight.Flights.Features.CreateFlight.Endpoints.V1;

using Hellang.Middleware.ProblemDetails;

public class CreateFlightEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{EndpointConfig.BaseApiPath}/flight", CreateFlight)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("CreateFlight")
            .WithMetadata(new SwaggerOperationAttribute("Create Flight", "Create Flight"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Flight").Build())
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status201Created,
                    "Flight Created",
                    typeof(FlightResponseDto)))
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

    private async Task<IResult> CreateFlight(CreateFlightRequestDto request, IMediator mediator, IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateFlightCommand>(request);

        var result = await mediator.Send(command, cancellationToken);

        return Results.CreatedAtRoute("GetFlightById", new {id = result.Id}, result);
    }
}
