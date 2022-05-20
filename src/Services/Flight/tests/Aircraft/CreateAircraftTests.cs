using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using FluentAssertions;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Aircraft;

[Collection(nameof(TestFixture))]
public class CreateAircraftTests
{
    private readonly TestFixture _fixture;

    public CreateAircraftTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task should_create_new_aircraft_to_db_and_publish_message_to_broker()
    {
        // Arrange
        var command = new FakeCreateAircraftCommand().Generate();

        // Act
        var aircraftResponse = await _fixture.SendAsync(command);

        // Assert
        aircraftResponse.Should().NotBeNull();
        aircraftResponse?.Name.Should().Be(command.Name);
        (await _fixture.IsFaultyPublished<AircraftCreated>()).Should().BeFalse();
        (await _fixture.IsPublished<AircraftCreated>()).Should().BeTrue();
    }
}
