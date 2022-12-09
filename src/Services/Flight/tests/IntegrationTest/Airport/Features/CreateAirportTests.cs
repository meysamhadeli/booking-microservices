using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase.IntegrationTest;
using Flight.Airports.Features.CreateAirport.Commands.V1.Reads;
using Flight.Api;
using Flight.Data;
using FluentAssertions;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Airport.Features;

public class CreateAirportTests : FlightIntegrationTestBase
{
    public CreateAirportTests(
        IntegrationTestFactory<Program, FlightDbContext, FlightReadDbContext> integrationTestFactory) : base(integrationTestFactory)
    {
    }

    [Fact]
    public async Task should_create_new_airport_to_db_and_publish_message_to_broker()
    {
        // Arrange
        var command = new FakeCreateAirportCommand().Generate();

        // Act
        var response = await Fixture.SendAsync(command);

        // Assert
        response?.Should().NotBeNull();
        response?.Name.Should().Be(command.Name);

        (await Fixture.WaitForPublishing<AirportCreated>()).Should().Be(true);

        (await Fixture.ShouldProcessedPersistInternalCommand<CreateAirportMongoCommand>()).Should().Be(true);
    }
}
