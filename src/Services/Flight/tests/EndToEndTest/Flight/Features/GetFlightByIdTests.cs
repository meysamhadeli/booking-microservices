using System.Net;
using BuildingBlocks.TestBase;
using EndToEnd.Test.Fakes;
using EndToEnd.Test.Routes;
using Flight.Api;
using Flight.Data;
using FluentAssertions;
using Xunit;

namespace EndToEnd.Test.Flight.Features;

using global::Flight.Flights.Features.CreatingFlight.V1;

public class GetFlightByIdTests: FlightEndToEndTestBase
{
    public GetFlightByIdTests(TestFixture<Program, FlightDbContext, FlightReadDbContext> integrationTestFixture) : base(integrationTestFixture)
    {
    }


    [Fact]
    public async Task should_retrive_a_flight_by_id_currectly()
    {
        //Arrange
        var command = new FakeCreateFlightCommand().Generate();
        await Fixture.SendAsync(command);
        (await Fixture.ShouldProcessedPersistInternalCommand<CreateFlightMongo>()).Should().Be(true);

        // Act
        var route = ApiRoutes.Flight.GetFlightById.Replace(ApiRoutes.Flight.Id, command.Id.ToString());
        var result = await Fixture.HttpClient.GetAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
