using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.TestBase.Auth;

//ref: https://blog.joaograssi.com/posts/2021/asp-net-core-testing-permission-protected-api-endpoints/
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{


    private readonly MockAuthUser _mockAuthUser;

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        MockAuthUser mockAuthUser)
        : base(options, logger, encoder, clock)
    {
        // 1. We get a "mock" user instance here via DI.
        // we'll see how this work later, don't worry
        _mockAuthUser = mockAuthUser;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (_mockAuthUser.Claims.Count == 0)
            return Task.FromResult(AuthenticateResult.Fail("Mock auth user not configured."));

        // 2. Create the principal and the ticket
        var identity = new ClaimsIdentity(_mockAuthUser.Claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        // 3. Authenticate the request
        var result = AuthenticateResult.Success(ticket);
        return Task.FromResult(result);
    }
}
