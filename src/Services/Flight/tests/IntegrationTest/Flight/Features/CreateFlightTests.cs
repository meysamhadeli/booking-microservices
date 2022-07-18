using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase;
using Flight.Data;
using Flight.Flights.Features.CreateFlight.Reads;
using FluentAssertions;
using Integration.Test.Fakes;
using MassTransit;
using MassTransit.Testing;
using Xunit;

namespace Integration.Test.Flight.Features;

public class CreateFlightTests : IntegrationTestBase<Program, FlightDbContext, FlightReadDbContext>
{
    private readonly ITestHarness _testHarness;

    public CreateFlightTests(
        IntegrationTestFixture<Program, FlightDbContext, FlightReadDbContext> integrationTestFixture) : base(
        integrationTestFixture)
    {
        _testHarness = Fixture.TestHarness;
    }

    [Fact]
    public async Task should_create_new_flight_to_db_and_publish_message_to_broker()
    {
        // Arrange
        var command = new FakeCreateFlightCommand().Generate();

        // Act
        var response = await Fixture.SendAsync(command);

        // Assert
        response.Should().NotBeNull();
        response?.FlightNumber.Should().Be(command.FlightNumber);

        (await _testHarness.Published.Any<Fault<FlightCreated>>()).Should().BeFalse();
        (await _testHarness.Published.Any<FlightCreated>()).Should().BeTrue();

        await Fixture.ShouldProcessedPersistInternalCommand<CreateFlightMongoCommand>();
    }
}
