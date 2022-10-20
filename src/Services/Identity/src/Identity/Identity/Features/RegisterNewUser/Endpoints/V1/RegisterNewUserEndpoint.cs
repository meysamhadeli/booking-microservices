using System.Threading;
using System.Threading.Tasks;
using Identity.Identity.Features.RegisterNewUser.Commands.V1;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Identity.Identity.Features.RegisterNewUser.Endpoints.V1;

[Route("identity/register-user")]
[ApiController]
public class LoginEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public LoginEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    // [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Register new user", Description = "Register new user")]
    public async Task<ActionResult> RegisterNewUser([FromBody] RegisterNewUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }
}
