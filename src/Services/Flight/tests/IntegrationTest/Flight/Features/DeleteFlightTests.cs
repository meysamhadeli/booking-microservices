using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase;
using Flight.Api;
using Flight.Data;
using Flight.Flights.Features.DeleteFlight.Commands.V1;
using Flight.Flights.Features.DeleteFlight.Commands.V1.Reads;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Integration.Test.Flight.Features;

public class DeleteFlightTests : FlightIntegrationTestBase
{
    public DeleteFlightTests(
        IntegrationTestFactory<Program, FlightDbContext, FlightReadDbContext> integrationTestFactory) : base(integrationTestFactory)
    {
    }

    [Fact]
    public async Task should_delete_flight_from_db()
    {
        // Arrange
        var flightEntity = await Fixture.FindAsync<global::Flight.Flights.Models.Flight>(1);
        var command = new DeleteFlightCommand(flightEntity.Id);

        // Act
        await Fixture.SendAsync(command);
        var deletedFlight = (await Fixture.ExecuteDbContextAsync(db => db.Flights
                .Where(x => x.Id == command.Id)
                .IgnoreQueryFilters()
                .ToListAsync())
            ).FirstOrDefault();

        // Assert
        deletedFlight?.IsDeleted.Should().BeTrue();

        (await Fixture.WaitForPublishing<FlightDeleted>()).Should().Be(true);

        (await Fixture.ShouldProcessedPersistInternalCommand<DeleteFlightMongoCommand>()).Should().Be(true);
    }
}
