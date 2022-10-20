using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Flights.Features.GetAvailableFlights.Queries.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Flight.Flights.Features.GetAvailableFlights.Endpoints.V1;

[Route(BaseApiPath + "/flight/get-available-flights")]
public class GetAvailableFlightsEndpoint : BaseController
{
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Get available flights", Description = "Get available flights")]
    public async Task<ActionResult> GetAvailableFlights([FromRoute] GetAvailableFlightsQuery query, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);

        return Ok(result);
    }
}
