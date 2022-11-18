using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase;
using Flight.Api;
using Flight.Data;
using Flight.Flights.Features.DeleteFlight;
using Flight.Flights.Features.DeleteFlight.Commands.V1;
using Flight.Flights.Features.DeleteFlight.Commands.V1.Reads;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Integration.Test.Flight.Features;

public class DeleteFlightTests : IntegrationTestBase<Program, FlightDbContext, FlightReadDbContext>
{
    private readonly ITestHarness _testHarness;

    public DeleteFlightTests(
        IntegrationTestFactory<Program, FlightDbContext, FlightReadDbContext> integrationTestFixture) : base(
        integrationTestFixture)
    {
        _testHarness = Fixture.TestHarness;
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
        (await _testHarness.Published.Any<Fault<FlightDeleted>>()).Should().BeFalse();
        (await _testHarness.Published.Any<FlightDeleted>()).Should().BeTrue();
        await Fixture.ShouldProcessedPersistInternalCommand<DeleteFlightMongoCommand>();
    }
}
