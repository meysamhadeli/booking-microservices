using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Jwt;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContext;

    public AuthHeaderHandler(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = (_httpContext?.HttpContext?.Request.Headers["Authorization"])?.ToString();

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token?.Replace("Bearer ", "", StringComparison.Ordinal));

        return base.SendAsync(request, cancellationToken);
    }
}
