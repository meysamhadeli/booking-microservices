using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using FluentAssertions;
using Integration.Test.Fakes;
using MassTransit;
using MassTransit.Testing;
using Xunit;

namespace Integration.Test.Aircraft.Features;
public class CreateAircraftTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;
    private readonly ITestHarness _testHarness;
    public CreateAircraftTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _testHarness = fixture.TestHarness;
    }

    [Fact]
    public async Task should_create_new_aircraft_to_db_and_publish_message_to_broker()
    {
        // Arrange
        var command = new FakeCreateAircraftCommand().Generate();

        // Act
        var response = await _fixture.SendAsync(command);

        // Assert
        response?.Should().NotBeNull();
        response?.Name.Should().Be(command.Name);
        (await _testHarness.Published.Any<Fault<AircraftCreated>>()).Should().BeFalse();
        (await _testHarness.Published.Any<AircraftCreated>()).Should().BeTrue();
    }
}
