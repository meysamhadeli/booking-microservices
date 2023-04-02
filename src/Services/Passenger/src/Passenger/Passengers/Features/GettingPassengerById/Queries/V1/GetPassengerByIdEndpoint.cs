namespace Passenger.Passengers.Features.GettingPassengerById.Queries.V1;

using BuildingBlocks.Web;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Dtos;
using Microsoft.OpenApi.Models;

public record GetPassengerByIdResponseDto(PassengerDto PassengerDto);

public class GetPassengerByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/passenger/{{id}}", GetById)
            .RequireAuthorization()
            .WithName("GetPassengerById")
            .WithApiVersionSet(builder.NewApiVersionSet("Passenger").Build())
            .Produces<GetPassengerByIdResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation => new OpenApiOperation(operation) { Summary = "Get Passenger By Id", Description = "Get Passenger By Id" })
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> GetById(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPassengerById(id), cancellationToken);

        var response = new GetPassengerByIdResponseDto(result?.PassengerDto);

        return Results.Ok(response);
    }
}
