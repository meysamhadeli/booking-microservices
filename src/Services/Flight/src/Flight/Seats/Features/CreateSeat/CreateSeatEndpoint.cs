using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Airports.Features.CreateAirport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Flight.Seats.Features.CreateSeat;

[Route(BaseApiPath + "/flight/seat")]
public class CreateSeatEndpoint : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Create new seat", Description = "Create new seat")]
    public async Task<ActionResult> Create(CreateSeatCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);

        return Ok(result);
    }
}
