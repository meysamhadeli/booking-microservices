namespace Flight.Flights.Features.DeletingFlight.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

public class DeleteFlightEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/flight/{{id}}", DeleteFlight)
            .RequireAuthorization()
            .WithName("DeleteFlight")
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation => new OpenApiOperation(operation) { Summary = "Delete Flight", Description = "Delete Flight" })
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> DeleteFlight(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteFlight(id), cancellationToken);

        return Results.NoContent();
    }
}
