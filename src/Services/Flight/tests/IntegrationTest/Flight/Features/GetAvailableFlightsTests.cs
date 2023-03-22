using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.TestBase;
using Flight.Api;
using Flight.Data;
using FluentAssertions;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Flight.Features;

using global::Flight.Flights.Features.CreatingFlight.V1;
using global::Flight.Flights.Features.GettingAvailableFlights.V1;

public class GetAvailableFlightsTests : FlightIntegrationTestBase
{
    public GetAvailableFlightsTests(
        TestFixture<Program, FlightDbContext, FlightReadDbContext> integrationTestFactory) : base(integrationTestFactory)
    {
    }

    [Fact]
    public async Task should_return_available_flights()
    {
        // Arrange
        var flightCommand = new FakeCreateFlightCommand().Generate();

        await Fixture.SendAsync(flightCommand);

        (await Fixture.ShouldProcessedPersistInternalCommand<CreateFlightMongo>()).Should().Be(true);

        var query = new GetAvailableFlights();

        // Act
        var response = (await Fixture.SendAsync(query))?.FlightDtos?.ToList();

        // Assert
        response?.Should().NotBeNull();
        response?.Count.Should().BeGreaterOrEqualTo(2);
    }
}
