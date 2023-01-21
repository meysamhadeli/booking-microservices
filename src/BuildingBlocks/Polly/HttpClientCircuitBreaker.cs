namespace BuildingBlocks.Polly;

using System.Net;
using Ardalis.GuardClauses;
using BuildingBlocks.Web;
using global::Polly;
using global::Polly.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Exception = System.Exception;

public static class HttpClientCircuitBreaker
{
    // ref: https://anthonygiretti.com/2019/03/26/best-practices-with-httpclient-and-retry-policies-with-polly-in-net-core-2-part-2/
    public static IHttpClientBuilder AddHttpClientCircuitBreakerPolicyHandler(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddPolicyHandler((sp, _) =>
        {
            var options = sp.GetRequiredService<IConfiguration>().GetOptions<PolicyOptions>(nameof(PolicyOptions));

            Guard.Against.Null(options, nameof(options));

            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("PollyHttpClientCircuitBreakerPoliciesLogger");

            return HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.BadRequest)
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: options.CircuitBreaker.RetryCount,
                    durationOfBreak: TimeSpan.FromSeconds(options.CircuitBreaker.BreakDuration),
                    onBreak: (response, breakDuration) =>
                    {
                        if (response?.Exception != null)
                        {
                            logger.LogError(response.Exception,
                                "Service shutdown during {BreakDuration} after {RetryCount} failed retries",
                                breakDuration,
                                options.CircuitBreaker.RetryCount);
                        }
                    },
                    onReset: () =>
                    {
                        logger.LogInformation("Service restarted");
                    });
        });
    }
}
