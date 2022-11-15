using Booking.Booking.Features.CreateBooking.Commands.V1;
using BuildingBlocks.Web;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace Booking.Booking.Features.CreateBooking.Endpoints.V1;
public class CreateBookingEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{EndpointConfig.BaseApiPath}/booking", CreateBooking)
            .RequireAuthorization()
            .WithTags("Booking")
            .WithName("Create Booking")
            .WithMetadata(new SwaggerOperationAttribute("Create Booking", "Create Booking"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Booking").Build())
            .Produces<ulong>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return endpoints;
    }

    private async Task<IResult> CreateBooking(CreateBookingCommand command, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}
