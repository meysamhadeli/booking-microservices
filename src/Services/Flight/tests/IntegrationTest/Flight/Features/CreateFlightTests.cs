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

public class CreateFlightTests : IntegrationTestBase<Program, FlightDbContext, FlightReadDbContext>
{
    public CreateFlightTests(
        IntegrationTestFixture<Program, FlightDbContext, FlightReadDbContext> integrationTestFixture) : base(
        integrationTestFixture)
    { }

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

        await Fixture.WaitForPublishing<FlightCreated>();
        await Fixture.WaitForConsuming<FlightCreated>();

        await Fixture.ShouldProcessedPersistInternalCommand<CreateFlightMongoCommand>();
    }
}
