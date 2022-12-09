using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase.IntegrationTest;
using Flight.Aircrafts.Features.CreateAircraft.Commands.V1.Reads;
using Flight.Api;
using Flight.Data;
using FluentAssertions;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Aircraft.Features;

public class CreateAircraftTests : FlightIntegrationTestBase
{
    public CreateAircraftTests(
        IntegrationTestFactory<Program, FlightDbContext, FlightReadDbContext> integrationTestFactory) : base(integrationTestFactory)
    {
    }

    [Fact]
    public async Task should_create_new_aircraft_to_db_and_publish_message_to_broker()
    {
        // Arrange
        var command = new FakeCreateAircraftCommand().Generate();

        // Act
        var response = await Fixture.SendAsync(command);

        // Assert
        response?.Should().NotBeNull();
        response?.Name.Should().Be(command.Name);

        (await Fixture.WaitForPublishing<AircraftCreated>()).Should().Be(true);

        (await Fixture.ShouldProcessedPersistInternalCommand<CreateAircraftMongoCommand>()).Should().Be(true);
    }
}
