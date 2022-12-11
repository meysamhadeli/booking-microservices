using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase;
using Flight.Api;
using Flight.Data;
using Flight.Flights.Features.UpdateFlight.Commands.V1.Reads;
using FluentAssertions;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Flight.Features;

public class UpdateFlightTests : FlightIntegrationTestBase
{
    public UpdateFlightTests(
        TestFixture<Program, FlightDbContext, FlightReadDbContext> integrationTestFactory) : base(integrationTestFactory)
    {
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

        (await Fixture.WaitForPublishing<FlightUpdated>()).Should().Be(true);

        (await Fixture.ShouldProcessedPersistInternalCommand<UpdateFlightMongoCommand>()).Should().Be(true);
    }
}
