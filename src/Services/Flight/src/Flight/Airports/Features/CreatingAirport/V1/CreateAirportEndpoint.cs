namespace Flight.Airports.Features.CreatingAirport.V1;

using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public record CreateAirportRequestDto(string Name, string Address, string Code);
public record CreateAirportResponseDto(long Id);

public class CreateAirportEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/flight/airport", CreateAirport)
            .RequireAuthorization()
            .WithMetadata(new SwaggerOperationAttribute("Create Airport", "Create Airport"))
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<CreateAirportResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> CreateAirport(CreateAirportRequestDto request, IMediator mediator, IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateAirport>(request);

        var result = await mediator.Send(command, cancellationToken);

        var response = new CreateAirportResponseDto(result.Id);

        return Results.Ok(response);
    }
}
