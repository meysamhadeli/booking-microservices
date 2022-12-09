using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase;
using Flight.Api;
using Flight.Data;
using Flight.Flights.Features.CreateFlight.Commands.V1.Reads;
using FluentAssertions;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Flight.Features;

public class CreateFlightTests : FlightIntegrationTestBase
{
    public CreateFlightTests(
        IntegrationTestFactory<Program, FlightDbContext, FlightReadDbContext> integrationTestFactory) : base(integrationTestFactory)
    {
    }

    [Fact]
    public async Task should_create_new_flight_to_db_and_publish_message_to_broker()
    {
        //Arrange
        var command = new FakeCreateFlightCommand().Generate();

        // Act
        var response = await Fixture.SendAsync(command);

        // Assert
        response.Should().NotBeNull();
        response?.FlightNumber.Should().Be(command.FlightNumber);

        (await Fixture.WaitForPublishing<FlightCreated>()).Should().Be(true);
        (await Fixture.WaitForConsuming<FlightCreated>()).Should().Be(true);

        (await Fixture.ShouldProcessedPersistInternalCommand<CreateFlightMongoCommand>()).Should().Be(true);
    }
}
