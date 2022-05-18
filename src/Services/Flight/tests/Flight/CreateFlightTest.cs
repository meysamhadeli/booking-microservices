using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using Flight.Flights.Features.CreateFlight;
using FluentAssertions;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Flight;

[Collection(nameof(TestFixture))]
public class CreateFlightTest
{
    private readonly TestFixture _fixture;

    public CreateFlightTest(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task should_create_new_flight_to_db()
    {
        // Arrange
        var fakeFlight = new FakeCreateFlightCommand().Generate();
        var command = new CreateFlightCommand(fakeFlight.FlightNumber, fakeFlight.AircraftId,
            fakeFlight.DepartureAirportId, fakeFlight.DepartureDate,
            fakeFlight.ArriveDate, fakeFlight.ArriveAirportId, fakeFlight.DurationMinutes, fakeFlight.FlightDate,
            fakeFlight.Status, fakeFlight.Price);

        // Act
        var flightResponse = await _fixture.SendAsync(command);

        // Assert
        flightResponse.Should().NotBeNull();
        flightResponse?.FlightNumber.Should().Be(command.FlightNumber);
        (await _fixture.IsConsumed<FlightCreated>()).Should().Be(true);
    }
}
