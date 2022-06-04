using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using FluentAssertions;
using Integration.Test.Fakes;
using MassTransit;
using MassTransit.Testing;
using Xunit;

namespace Integration.Test.Flight.Features;

[Collection(nameof(IntegrationTestFixture))]
public class UpdateFlightTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly ITestHarness _testHarness;

    public UpdateFlightTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _testHarness = _fixture.TestHarness;
    }

    [Fact]
    public async Task should_update_flight_to_db_and_publish_message_to_broker()
    {
        // Arrange
        var fakeCreateCommandFlight = new FakeCreateFlightCommand().Generate();
        var flightEntity = FakeFlightCreated.Generate(fakeCreateCommandFlight);
        await _fixture.InsertAsync(flightEntity);

        var command = new FakeUpdateFlightCommand(flightEntity.Id).Generate();

        // Act
        var response = await _fixture.SendAsync(command);

        // Assert
        response.Should().NotBeNull();
        response?.Id.Should().Be(flightEntity?.Id);
        response?.Price.Should().NotBe(flightEntity?.Price);
        (await _testHarness.Published.Any<Fault<FlightUpdated>>()).Should().BeFalse();
        (await _testHarness.Published.Any<FlightUpdated>()).Should().BeTrue();
    }
}
