using System.Linq;
using System.Threading.Tasks;
using Flight.Flights.Features.GetAvailableFlights;
using FluentAssertions;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Flight.Features;

[Collection(nameof(TestFixture))]
public class GetAvailableFlightsTests
{
    private readonly TestFixture _fixture;

    public GetAvailableFlightsTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task should_return_available_flights()
    {
        // Arrange
        var flightCommand1 = new FakeCreateFlightCommand().Generate();
        var flightCommand2 = new FakeCreateFlightCommand().Generate();

        var flightEntity1 = global::Flight.Flights.Models.Flight.Create(
            flightCommand1.Id, flightCommand1.FlightNumber, flightCommand1.AircraftId, flightCommand1.DepartureAirportId, flightCommand1.DepartureDate,
            flightCommand1.ArriveDate, flightCommand1.ArriveAirportId, flightCommand1.DurationMinutes, flightCommand1.FlightDate, flightCommand1.Status, flightCommand1.Price);

        var flightEntity2 = global::Flight.Flights.Models.Flight.Create(
            flightCommand2.Id, flightCommand2.FlightNumber, flightCommand2.AircraftId, flightCommand2.DepartureAirportId, flightCommand2.DepartureDate,
            flightCommand2.ArriveDate, flightCommand2.ArriveAirportId, flightCommand2.DurationMinutes, flightCommand2.FlightDate, flightCommand2.Status, flightCommand2.Price);

        await _fixture.InsertAsync(flightEntity1, flightEntity2);

        var query = new GetAvailableFlightsQuery();

        // Act
        var response = (await _fixture.SendAsync(query))?.ToList();

        // Assert
        response?.Should().NotBeNull();
        response?.Count().Should().BeGreaterOrEqualTo(2);
    }
}
