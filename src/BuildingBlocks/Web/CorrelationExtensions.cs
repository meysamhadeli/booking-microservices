using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web;

public static class CorrelationExtensions
{
    private const string CorrelationId = "correlationId";

    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.Use(async (ctx, next) =>
        {
            if (!ctx.Request.Headers.TryGetValue(CorrelationId, out var correlationId))
                correlationId = Guid.NewGuid().ToString("N");

            ctx.Items[CorrelationId] = correlationId.ToString();
            await next();
        });
    }

    public static string GetCorrelationId(this HttpContext context)
    {
        return context.Items.TryGetValue(CorrelationId, out var correlationId) ? correlationId as string : null;
    }
}
