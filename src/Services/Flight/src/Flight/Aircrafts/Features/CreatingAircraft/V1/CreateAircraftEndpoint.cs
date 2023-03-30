namespace Flight.Aircrafts.Features.CreatingAircraft.V1;

using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public record CreateAircraftRequestDto(string Name, string Model, int ManufacturingYear);
public record CreateAircraftResponseDto(long Id);

public class CreateAircraftEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/flight/aircraft", CreateAircraft)
            .RequireAuthorization()
            .WithName("CreateAircraft")
            .WithMetadata(new SwaggerOperationAttribute("Create Aircraft", "Create Aircraft"))
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<CreateAircraftResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> CreateAircraft(CreateAircraftRequestDto request, IMediator mediator, IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateAircraft>(request);

        var result = await mediator.Send(command, cancellationToken);

        var response = new CreateAircraftResponseDto(result.Id);

        return Results.Ok(response);
    }
}
