using System.Threading.Tasks;
using BuildingBlocks.TestBase;
using Flight;
using Flight.Api;
using Flight.Data;
using FluentAssertions;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Seat.Features;

using global::Flight.Flights.Features.CreatingFlight.V1;
using global::Flight.Seats.Features.CreatingSeat.V1;

public class ReserveSeatTests : FlightIntegrationTestBase
{
    public ReserveSeatTests(
        TestFixture<Program, FlightDbContext, FlightReadDbContext> integrationTestFactory) : base(integrationTestFactory)
    {
    }

    [Fact]
    public async Task should_return_valid_reserve_seat_from_grpc_service()
    {
        // Arrange
        var flightCommand = new FakeCreateFlightCommand().Generate();

        await Fixture.SendAsync(flightCommand);

        (await Fixture.ShouldProcessedPersistInternalCommand<CreateFlightMongo>()).Should().Be(true);

        var seatCommand = new FakeCreateSeatCommand(flightCommand.Id).Generate();

        await Fixture.SendAsync(seatCommand);

        (await Fixture.ShouldProcessedPersistInternalCommand<CreateSeatMongo>()).Should().Be(true);

        var flightGrpcClient = new FlightGrpcService.FlightGrpcServiceClient(Fixture.Channel);

        // Act
        var response = await flightGrpcClient.ReserveSeatAsync(new ReserveSeatRequest()
        {
            FlightId = seatCommand.FlightId.ToString(), SeatNumber = seatCommand.SeatNumber
        });

        // Assert
        response?.Should().NotBeNull();
        response?.Id.Should().Be(seatCommand.Id.ToString());
    }
}
