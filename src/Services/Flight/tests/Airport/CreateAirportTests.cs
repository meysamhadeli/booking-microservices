using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using FluentAssertions;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Airport;

[Collection(nameof(TestFixture))]
public class CreateAirportTests
{
    private readonly TestFixture _fixture;

    public CreateAirportTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task should_create_new_airport_to_db_and_publish_message_to_broker()
    {
        // Arrange
        var command = new FakeCreateAirportCommand().Generate();

        // Act
        var airportResponse = await _fixture.SendAsync(command);

        // Assert
        airportResponse.Should().NotBeNull();
        airportResponse?.Name.Should().Be(command.Name);
        (await _fixture.IsFaultyPublished<AirportCreated>()).Should().BeFalse();
        (await _fixture.IsPublished<AirportCreated>()).Should().BeTrue();
    }
}
