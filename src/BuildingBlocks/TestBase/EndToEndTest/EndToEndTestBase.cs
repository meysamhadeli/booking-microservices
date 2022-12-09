using System.Net.Http.Json;
using BuildingBlocks.Mongo;
using BuildingBlocks.TestBase.IntegrationTest;
using BuildingBlocks.Web;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace BuildingBlocks.TestBase.EndToEndTest;

public class EndToEndTestBase<TEntryPoint, TWContext, TRContext> :
    IntegrationTestBase<TEntryPoint, TWContext, TRContext>
    where TWContext : DbContext
    where TRContext : MongoDbContext
    where TEntryPoint : class
{
    public EndToEndTestBase(
        IntegrationTestFactory<TEntryPoint, TWContext, TRContext> sharedFixture,
        ITestOutputHelper outputHelper = null)
        : base(sharedFixture, outputHelper)
    {
    }

    public async Task<TResponse> GetAsync<TResponse>(string requestUrl, CancellationToken cancellationToken = default)
    {
        return await Fixture.HttpClient.GetFromJsonAsync<TResponse>(requestUrl, cancellationToken: cancellationToken);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string requestUrl, TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await Fixture.HttpClient.PostAsJsonAsync<TRequest, TResponse>(requestUrl, request, cancellationToken);
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(
        string requestUrl,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await Fixture.HttpClient.PutAsJsonAsync<TRequest, TResponse>(requestUrl, request, cancellationToken);
    }

    public async Task Delete(string requestUrl, CancellationToken cancellationToken = default)
    {
        await Fixture.HttpClient.DeleteAsync(requestUrl, cancellationToken);
    }
}
