using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using FluentAssertions;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Flight;

[Collection(nameof(TestFixture))]
public class CreateFlightTests
{
    private readonly TestFixture _fixture;

    public CreateFlightTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task should_create_new_flight_to_db_and_publish_message_to_broker()
    {
        // Arrange
        var command = new FakeCreateFlightCommand().Generate();

        // Act
        var flightResponse = await _fixture.SendAsync(command);

        // Assert
        flightResponse.Should().NotBeNull();
        flightResponse?.FlightNumber.Should().Be(command.FlightNumber);
        (await _fixture.IsFaultyPublished<FlightCreated>()).Should().BeFalse();
        (await _fixture.IsPublished<FlightCreated>()).Should().BeTrue();
    }
}
