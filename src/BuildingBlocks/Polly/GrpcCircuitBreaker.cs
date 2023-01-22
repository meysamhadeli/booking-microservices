namespace BuildingBlocks.Polly;

using System.Net;
using Ardalis.GuardClauses;
using BuildingBlocks.Web;
using global::Polly;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class GrpcCircuitBreaker
{
    //ref: https://anthonygiretti.com/2020/03/31/grpc-asp-net-core-3-1-resiliency-with-polly/
    public static IHttpClientBuilder AddGrpcCircuitBreakerPolicyHandler(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddPolicyHandler((sp, _) =>
        {
            var options = sp.GetRequiredService<IConfiguration>().GetOptions<PolicyOptions>(nameof(PolicyOptions));

            Guard.Against.Null(options, nameof(options));

            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("PollyGrpcCircuitBreakerPoliciesLogger");

            return Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
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
