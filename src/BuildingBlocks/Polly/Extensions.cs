namespace BuildingBlocks.Polly;

using global::Polly;
using Serilog;
using Exception = System.Exception;

public static class Extensions
{
    public static T RetryOnFailure<T>(this object retrySource, Func<T> action, int retryCount = 3)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .Retry(retryCount, (exception, retryAttempt, context) =>
            {
                Log.Information($"Retry attempt: {retryAttempt}");
                Log.Error($"Exception: {exception.Message}");
            });

        return retryPolicy.Execute(action);
    }
}
