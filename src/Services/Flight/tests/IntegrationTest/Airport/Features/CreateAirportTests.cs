using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using FluentAssertions;
using Integration.Test.Fakes;
using MassTransit;
using MassTransit.Testing;
using Xunit;

namespace Integration.Test.Airport.Features;

[Collection(nameof(TestFixture))]
public class CreateAirportTests
{
    private readonly TestFixture _fixture;
    private readonly ITestHarness _testHarness;

    public CreateAirportTests(TestFixture fixture)
    {
        _fixture = fixture;
        _testHarness = fixture.TestHarness;
    }

    [Fact]
    public async Task should_create_new_airport_to_db_and_publish_message_to_broker()
    {
        // Arrange
        var command = new FakeCreateAirportCommand().Generate();

        // Act
        var response = await _fixture.SendAsync(command);

        // Assert
        response?.Should().NotBeNull();
        response?.Name.Should().Be(command.Name);
        (await _testHarness.Published.Any<Fault<AirportCreated>>()).Should().BeFalse();
        (await _testHarness.Published.Any<AirportCreated>()).Should().BeTrue();
    }
}
