using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase;
using Flight.Api;
using Flight.Data;
using FluentAssertions;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Airport.Features;

using global::Flight.Airports.Features.CreatingAirport.V1;

public class CreateAirportTests : FlightIntegrationTestBase
{
    public CreateAirportTests(
        TestFixture<Program, FlightDbContext, FlightReadDbContext> integrationTestFactory) : base(integrationTestFactory)
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
        response?.Id.Should().Be(command.Id);

        (await Fixture.WaitForPublishing<AirportCreated>()).Should().Be(true);

        (await Fixture.ShouldProcessedPersistInternalCommand<CreateAirportMongo>()).Should().Be(true);
    }
}
