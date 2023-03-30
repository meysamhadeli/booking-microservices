namespace Flight.Flights.Features.DeletingFlight.V1;

using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public class DeleteFlightEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/flight/{{id}}", DeleteFlight)
            .RequireAuthorization()
            .WithName("DeleteFlight")
            .WithMetadata(new SwaggerOperationAttribute("Delete Flight", "Delete Flight"))
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> DeleteFlight(long id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteFlight(id), cancellationToken);

        return Results.NoContent();
    }
}
