using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase;
using Flight.Api;
using Flight.Data;
using Flight.Flights.Features.UpdateFlight.Commands.V1.Reads;
using FluentAssertions;
using Integration.Test.Fakes;
using MassTransit;
using MassTransit.Testing;
using Xunit;

namespace Integration.Test.Flight.Features;
public class UpdateFlightTests : IntegrationTestBase<Program, FlightDbContext, FlightReadDbContext>
{
    private readonly ITestHarness _testHarness;

    public UpdateFlightTests(IntegrationTestFactory<Program, FlightDbContext, FlightReadDbContext> integrationTestFixture) : base(integrationTestFixture)
    {
        _testHarness = Fixture.TestHarness;
    }


    [Fact]
    public async Task should_update_flight_to_db_and_publish_message_to_broker()
    {
        // Arrange
        var flightEntity = await Fixture.FindAsync<global::Flight.Flights.Models.Flight>(1);
        var command = new FakeUpdateFlightCommand(flightEntity).Generate();

        // Act
        var response = await Fixture.SendAsync(command);

        // Assert
        response.Should().NotBeNull();
        response?.Id.Should().Be(flightEntity?.Id);
        response?.Price.Should().NotBe(flightEntity?.Price);
        (await _testHarness.Published.Any<Fault<FlightUpdated>>()).Should().BeFalse();
        (await _testHarness.Published.Any<FlightUpdated>()).Should().BeTrue();
        await Fixture.ShouldProcessedPersistInternalCommand<UpdateFlightMongoCommand>();
    }
}
