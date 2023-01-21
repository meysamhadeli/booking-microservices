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

            // gRPC status
            var gRpcErrors = new StatusCode[]
            {
                StatusCode.DeadlineExceeded, StatusCode.Internal, StatusCode.NotFound, StatusCode.Cancelled,
                StatusCode.ResourceExhausted, StatusCode.Unavailable, StatusCode.Unknown
            };

            // Http errors
            var serverErrors = new HttpStatusCode[]
            {
                HttpStatusCode.BadGateway, HttpStatusCode.GatewayTimeout, HttpStatusCode.ServiceUnavailable,
                HttpStatusCode.InternalServerError, HttpStatusCode.TooManyRequests, HttpStatusCode.RequestTimeout
            };

            return Policy.HandleResult<HttpResponseMessage>(r =>
                {
                    var grpcStatus = StatusManager.GetStatusCode(r);
                    var httpStatusCode = r.StatusCode;

                    return (grpcStatus == null && serverErrors.Contains(httpStatusCode)) || // if the server send an error before gRPC pipeline
                           (httpStatusCode == HttpStatusCode.OK && gRpcErrors.Contains(grpcStatus.Value)); // if gRPC pipeline handled the request (gRPC always answers OK)
                })
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

    private static class StatusManager
    {
        public static StatusCode? GetStatusCode(HttpResponseMessage response)
        {
            var headers = response.Headers;

            if (!headers.Contains("grpc-status") && response.StatusCode == HttpStatusCode.OK)
                return StatusCode.OK;

            if (headers.Contains("grpc-status"))
                return (StatusCode)int.Parse(headers.GetValues("grpc-status").First());

            return null;
        }
    }
}
