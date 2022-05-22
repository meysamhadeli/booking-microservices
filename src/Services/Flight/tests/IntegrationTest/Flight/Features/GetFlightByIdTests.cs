using System.Threading.Tasks;
using Flight.Flights.Features.GetFlightById;
using FluentAssertions;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Flight.Features;

[Collection(nameof(TestFixture))]
public class GetFlightByIdTests
{
    private readonly TestFixture _fixture;

    public GetFlightByIdTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task should_retrive_a_flight_by_id_currectly()
    {
        // Arrange
        var command = new FakeCreateFlightCommand().Generate();
        var flightEntity = global::Flight.Flights.Models.Flight.Create(
            command.Id, command.FlightNumber, command.AircraftId, command.DepartureAirportId, command.DepartureDate,
            command.ArriveDate, command.ArriveAirportId, command.DurationMinutes, command.FlightDate, command.Status, command.Price);
        await _fixture.InsertAsync(flightEntity);

        var query = new GetFlightByIdQuery(flightEntity.Id);

        // Act
        var response = await _fixture.SendAsync(query);

        // Assert
        response.Should().NotBeNull();
        response?.Id.Should().Be(flightEntity.Id);
    }
}
