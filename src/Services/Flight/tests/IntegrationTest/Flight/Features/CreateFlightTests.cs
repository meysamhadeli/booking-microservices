using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using FluentAssertions;
using Integration.Test.Fakes;
using MassTransit;
using MassTransit.Testing;
using Xunit;

namespace Integration.Test.Flight.Features;

[Collection(nameof(IntegrationTestFixture))]
public class CreateFlightTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly ITestHarness _testHarness;

    public CreateFlightTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _testHarness = _fixture.TestHarness;
    }

    [Fact]
    public async Task should_create_new_flight_to_db_and_publish_message_to_broker()
    {
        // Arrange
        var command = new FakeCreateFlightCommand().Generate();

        // Act
        var response = await _fixture.SendAsync(command);

        // Assert
        response.Should().NotBeNull();
        response?.FlightNumber.Should().Be(command.FlightNumber);
        (await _testHarness.Published.Any<Fault<FlightCreated>>()).Should().BeFalse();
        (await _testHarness.Published.Any<FlightCreated>()).Should().BeTrue();
    }
}
