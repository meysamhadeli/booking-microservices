using System.Net.Http.Json;

namespace BuildingBlocks.Web;

public static class HttpClientExtensions
{
    public static async Task<TResponse>
        PostAsJsonAsync<TRequest, TResponse>(
            this HttpClient httpClient,
            string requestUri,
            TRequest request,
            CancellationToken cancellationToken = default)
    {
        var responseMessage =
            await httpClient.PostAsJsonAsync(requestUri, request, cancellationToken: cancellationToken);

        var result = await responseMessage.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);

        return result;
    }

    public static async Task<TResponse?>
        PutAsJsonAsync<TRequest, TResponse>(
            this HttpClient httpClient,
            string requestUri,
            TRequest request,
            CancellationToken cancellationToken = default)
    {
        var responseMessage =
            await httpClient.PutAsJsonAsync(requestUri, request, cancellationToken: cancellationToken);

        var result = await responseMessage.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);

        return result;
    }
}
