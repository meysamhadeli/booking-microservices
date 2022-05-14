using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using Flight.Airports.Models;
using Flight.Flights.Features.GetFlightById;
using MassTransit.Testing;
using Xunit;

namespace Integration.Test;

[Collection(nameof(TestFixture))]
public class DeleteTests
{
    private readonly TestFixture _fixture;

    public DeleteTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Should_get_flight()
    {
        var query = new GetFlightByIdQuery(1);
        var flight = await _fixture.SendAsync(query);
        var airport = await _fixture.FindAsync<Airport>(1);
    }
}
