using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Airports.Dtos;
using Flight.Airports.Features.CreateAirport.Commands.V1;
using Flight.Airports.Features.CreateAirport.Dtos.V1;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace Flight.Airports.Features.CreateAirport.Endpoints.V1;

using Hellang.Middleware.ProblemDetails;

public class CreateAirportEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{EndpointConfig.BaseApiPath}/flight/airport", CreateAirport)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("CreateAirport")
            .WithMetadata(new SwaggerOperationAttribute("Create Airport", "Create Airport"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Flight").Build())
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status200OK,
                    "Airport Created",
                    typeof(AirportResponseDto)))
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

    private async Task<IResult> CreateAirport(CreateAirportRequestDto request, IMediator mediator, IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateAirportCommand>(request);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}
