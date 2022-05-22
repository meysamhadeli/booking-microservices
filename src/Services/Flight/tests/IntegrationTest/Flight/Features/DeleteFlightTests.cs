using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using Flight.Flights.Features.DeleteFlight;
using FluentAssertions;
using Integration.Test.Fakes;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Integration.Test.Flight.Features;

[Collection(nameof(TestFixture))]
public class DeleteFlightTests
{
    private readonly TestFixture _fixture;
    private readonly ITestHarness _testHarness;

    public DeleteFlightTests(TestFixture fixture)
    {
        _fixture = fixture;
        _testHarness = fixture.TestHarness;
    }

    [Fact]
    public async Task should_delete_flight_from_db()
    {
        // Arrange
        var createFlightCommand = new FakeCreateFlightCommand().Generate();
        var flightEntity = global::Flight.Flights.Models.Flight.Create(
            createFlightCommand.Id, createFlightCommand.FlightNumber, createFlightCommand.AircraftId, createFlightCommand.DepartureAirportId,
            createFlightCommand.DepartureDate, createFlightCommand.ArriveDate, createFlightCommand.ArriveAirportId, createFlightCommand.DurationMinutes,
            createFlightCommand.FlightDate, createFlightCommand.Status, createFlightCommand.Price);
        await _fixture.InsertAsync(flightEntity);

        var command = new DeleteFlightCommand(flightEntity.Id);

        // Act
        await _fixture.SendAsync(command);
        var deletedFlight = (await _fixture.ExecuteDbContextAsync(db => db.Flights
                .Where(x => x.Id == command.Id)
                .IgnoreQueryFilters()
                .ToListAsync())
            ).FirstOrDefault();

        // Assert
        deletedFlight?.IsDeleted.Should().BeTrue();
        (await _testHarness.Published.Any<Fault<FlightDeleted>>()).Should().BeFalse();
        (await _testHarness.Published.Any<FlightDeleted>()).Should().BeTrue();
    }
}

