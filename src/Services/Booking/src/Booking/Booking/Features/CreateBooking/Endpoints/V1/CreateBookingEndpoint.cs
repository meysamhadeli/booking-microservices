using Booking.Booking.Features.CreateBooking.Commands.V1;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Booking.Booking.Features.CreateBooking.Endpoints.V1;

[Route(BaseApiPath + "/booking")]
public class CreateBookingEndpoint : BaseController
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Create new Reservation", Description = "Create new Reservation")]
    public async Task<ActionResult> CreateReservation([FromBody] CreateBookingCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);

        return Ok(result);
    }
}
