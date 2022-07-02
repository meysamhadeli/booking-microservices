using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.Grpc;
using Flight.Flights.Features.GetAvailableFlights;
using FluentAssertions;
using Grpc.Net.Client;
using Integration.Test.Fakes;
using MagicOnion.Client;
using Xunit;

namespace Integration.Test.Flight.Features;

public class GetAvailableFlightsTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;
    private readonly GrpcChannel _channel;

    public GetAvailableFlightsTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _channel = fixture.Channel;
    }

    [Fact]
    public async Task should_return_available_flights()
    {
        // Arrange
        var flightCommand1 = new FakeCreateFlightCommand().Generate();
        var flightCommand2 = new FakeCreateFlightCommand().Generate();

        var flightEntity1 = FakeFlightCreated.Generate(flightCommand1);
        var flightEntity2 = FakeFlightCreated.Generate(flightCommand2);

        await _fixture.InsertAsync(flightEntity1, flightEntity2);

        var query = new GetAvailableFlightsQuery();

        // Act
        var response = (await _fixture.SendAsync(query))?.ToList();

        // Assert
        response?.Should().NotBeNull();
        response?.Count().Should().BeGreaterOrEqualTo(2);
    }
}
