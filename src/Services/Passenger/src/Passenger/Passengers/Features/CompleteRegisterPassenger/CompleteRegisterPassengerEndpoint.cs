using BuildingBlocks.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Passenger.Passengers.Features.CompleteRegisterPassenger;

[Route(BaseApiPath + "/passenger/complete-registration")]
public class CompleteRegisterPassengerEndpoint : BaseController
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Complete Register Passenger", Description = "Complete Register Passenger")]
    public async Task<ActionResult> CompleteRegisterPassenger([FromBody] CompleteRegisterPassengerCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);

        return Ok(result);
    }
}
