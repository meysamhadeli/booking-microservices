namespace BuildingBlocks.Polly;

using global::Polly;
using Exception = System.Exception;

public static class Extensions
{
    public static T RetryOnFailure<T>(this object retrySource, Func<T> action, int retryCount = 3)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .Retry(retryCount, (exception, retryAttempt) =>
            {
                Console.WriteLine($"Retry attempt: {retryAttempt}");
                Console.WriteLine($"Exception: {exception.Message}");
            });

        return retryPolicy.Execute(action);
    }
}
