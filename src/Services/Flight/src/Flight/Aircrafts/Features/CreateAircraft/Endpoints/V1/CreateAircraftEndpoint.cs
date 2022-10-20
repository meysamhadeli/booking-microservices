using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Aircrafts.Features.CreateAircraft.Commands.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Flight.Aircrafts.Features.CreateAircraft.Endpoints.V1;

[Route(BaseApiPath + "/flight/aircraft")]
public class CreateAircraftEndpoint : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Create new aircraft", Description = "Create new aircraft")]
    public async Task<ActionResult> Create([FromBody] CreateAircraftCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);

        return Ok(result);
    }
}
