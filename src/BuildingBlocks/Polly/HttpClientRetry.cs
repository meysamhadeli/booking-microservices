namespace BuildingBlocks.Polly;

using System.Net;
using Ardalis.GuardClauses;
using BuildingBlocks.Web;
using global::Polly;
using global::Polly.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class HttpClientRetry
{
    // ref: https://anthonygiretti.com/2019/03/26/best-practices-with-httpclient-and-retry-policies-with-polly-in-net-core-2-part-2/
    public static IHttpClientBuilder AddHttpClientRetryPolicyHandler(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddPolicyHandler((sp, _) =>
        {
            var options = sp.GetRequiredService<IConfiguration>().GetOptions<PolicyOptions>(nameof(PolicyOptions));

            Guard.Against.Null(options, nameof(options));

            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("PollyHttpClientRetryPoliciesLogger");

            return HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.BadRequest)
                .OrResult(msg => msg.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetryAsync(retryCount: options.Retry.RetryCount,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(options.Retry.SleepDuration),
                    onRetry: (response, timeSpan, retryCount, context) =>
                    {
                        if (response?.Exception != null)
                        {
                            logger.LogError(response.Exception,
                                "Request failed with {StatusCode}. Waiting {TimeSpan} before next retry. Retry attempt {RetryCount}.",
                                response.Result.StatusCode,
                                timeSpan,
                                retryCount);
                        }
                    });
        });
    }
}
