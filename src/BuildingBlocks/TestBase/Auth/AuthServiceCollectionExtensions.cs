using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.TestBase.Auth;

//ref: https://blog.joaograssi.com/posts/2021/asp-net-core-testing-permission-protected-api-endpoints/
public static class AuthServiceCollectionExtensions
{
    private static MockAuthUser GetMockUser() =>
        new MockAuthUser(new Claim("sub", Guid.NewGuid().ToString()),
            new Claim("email", "sam@test.com"));

    public static AuthenticationBuilder AddTestAuthentication(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // AuthConstants.Scheme is just a scheme we define. I called it "TestAuth"
            options.DefaultPolicy = new AuthorizationPolicyBuilder("Test")
                .RequireAuthenticatedUser()
                .Build();
        });

        // Register a default user, so all requests have it by default
        services.AddScoped(_ => GetMockUser());

        // Register our custom authentication handler
        return services.AddAuthentication("Test")
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
    }
}
