namespace Flight.Aircrafts.Features.CreatingAircraft.V1;

using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Dtos;
using Hellang.Middleware.ProblemDetails;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public record CreateAircraftRequestDto(string Name, string Model, int ManufacturingYear);

public class CreateAircraftEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/flight/aircraft", CreateAircraft)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("CreateAircraft")
            .WithMetadata(new SwaggerOperationAttribute("Create Aircraft", "Create Aircraft"))
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status200OK,
                    "Aircraft Created",
                    typeof(AircraftDto)))
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

    private async Task<IResult> CreateAircraft(CreateAircraftRequestDto request, IMediator mediator, IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateAircraft>(request);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}
