// using System.Threading;
// using System.Threading.Tasks;
// using BuildingBlocks.Web;
// using Flight.Seats.Features.CreateSeat.Commands.V1;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Swashbuckle.AspNetCore.Annotations;
//
// namespace Flight.Seats.Features.CreateSeat.Endpoints.V1;
//
// [Route(BaseApiPath + "/flight/seat")]
// public class CreateSeatEndpoint : BaseController
// {
//     [HttpPost]
//     [ProducesResponseType(StatusCodes.Status201Created)]
//     [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     [SwaggerOperation(Summary = "Create new seat", Description = "Create new seat")]
//     public async Task<ActionResult> Create(CreateSeatCommand command, CancellationToken cancellationToken)
//     {
//         var result = await Mediator.Send(command, cancellationToken);
//
//         return Ok(result);
//     }
// }


using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Seats.Dtos;
using Flight.Seats.Features.CreateSeat.Commands.V1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public class CreateSeatEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{EndpointConfig.BaseApiPath}/flight/seat", CreateSeat)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("Create Seat")
            .WithMetadata(new SwaggerOperationAttribute("Create Seat", "Create Seat"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Flight").Build())
            .Produces<SeatResponseDto>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return endpoints;
    }

    private async Task<IResult> CreateSeat(CreateSeatCommand command, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}
