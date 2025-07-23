using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Polly;

using global::Polly;
using Exception = System.Exception;

public static class Extensions
{
    public static ILogger Logger { get; set; } = null!;

    public static T RetryOnFailure<T>(this object retrySource, Func<T> action, int retryCount = 3)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .Retry(retryCount, (exception, retryAttempt, context) =>
                               {
                                   Logger.LogInformation($"Retry attempt: {retryAttempt}");
                                   Logger.LogError($"Exception: {exception.Message}");
                               });

        return retryPolicy.Execute(action);
    }
}