namespace Flight.Airports.Features.CreatingAirport.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

public record CreateAirportRequestDto(string Name, string Address, string Code);
public record CreateAirportResponseDto(Guid Id);

public class CreateAirportEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/flight/airport", CreateAirport)
            .RequireAuthorization()
            .WithName("CreateAirport")
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<CreateAirportResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation => new OpenApiOperation(operation) { Summary = "Create Airport", Description = "Create Airport" })
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
