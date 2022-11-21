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

public class CreateAirportEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{EndpointConfig.BaseApiPath}/flight/airport", CreateAirport)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("Create Airport")
            .WithMetadata(new SwaggerOperationAttribute("Create Airport", "Create Airport"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Flight").Build())
            .Produces<AirportResponseDto>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
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
