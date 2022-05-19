using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using Flight.Flights.Features.CreateFlight;
using FluentAssertions;
using Integration.Test.Fakes;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Integration.Test.Flight;

[Collection(nameof(TestFixture))]
public class UpdateFlightTests
{
    private readonly TestFixture _fixture;

    public UpdateFlightTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task should_update_flight_to_db_and_publish_message_to_broker()
    {
        // Arrange
        var fakeCreateCommandFlight = new FakeCreateFlightCommand().Generate();
        var flightEntity = global::Flight.Flights.Models.Flight.Create(fakeCreateCommandFlight.Id, fakeCreateCommandFlight.FlightNumber,
            fakeCreateCommandFlight.AircraftId, fakeCreateCommandFlight.DepartureAirportId, fakeCreateCommandFlight.DepartureDate,
            fakeCreateCommandFlight.ArriveDate, fakeCreateCommandFlight.ArriveAirportId, fakeCreateCommandFlight.DurationMinutes,
            fakeCreateCommandFlight.FlightDate, fakeCreateCommandFlight.Status, fakeCreateCommandFlight.Price);
        await _fixture.InsertAsync(flightEntity);

        var command = new FakeUpdateFlightCommand(flightEntity.Id).Generate();

        // Act
        var flightResponse = await _fixture.SendAsync(command);

        // Assert
        flightResponse.Should().NotBeNull();
        flightResponse?.Id.Should().Be(flightEntity?.Id);
        flightResponse?.Price.Should().NotBe(flightEntity?.Price);
        (await _fixture.IsFaultyPublished<FlightUpdated>()).Should().BeFalse();
        (await _fixture.IsPublished<FlightUpdated>()).Should().BeTrue();
    }
}
