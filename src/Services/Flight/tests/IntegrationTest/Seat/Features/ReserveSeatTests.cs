using System.Threading.Tasks;
using BuildingBlocks.TestBase;
using Flight;
using Flight.Api;
using Flight.Data;
using Flight.Flights.Features.CreateFlight.Commands.V1.Reads;
using Flight.Seats.Features.CreateSeat.Commands.V1.Reads;
using FluentAssertions;
using Grpc.Net.Client;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Seat.Features;

public class ReserveSeatTests : IntegrationTestBase<Program, FlightDbContext, FlightReadDbContext>
{
    private readonly GrpcChannel _channel;

    public ReserveSeatTests(
        IntegrationTestFactory<Program, FlightDbContext, FlightReadDbContext> integrationTestFixture) : base(
        integrationTestFixture)
    {
        _channel = Fixture.Channel;
    }

    [Fact]
    public async Task should_return_valid_reserve_seat_from_grpc_service()
    {
        // Arrange
        var flightCommand = new FakeCreateFlightCommand().Generate();

        await Fixture.SendAsync(flightCommand);

        await Fixture.ShouldProcessedPersistInternalCommand<CreateFlightMongoCommand>();

        var seatCommand = new FakeCreateSeatCommand(flightCommand.Id).Generate();

        await Fixture.SendAsync(seatCommand);

        await Fixture.ShouldProcessedPersistInternalCommand<CreateSeatMongoCommand>();

        var flightGrpcClient = new FlightGrpcService.FlightGrpcServiceClient(_channel);

        // Act
        var response = await flightGrpcClient.ReserveSeatAsync(new ReserveSeatRequest()
        {
            FlightId = seatCommand.FlightId, SeatNumber = seatCommand.SeatNumber
        });

        // Assert
        response?.Should().NotBeNull();
        response?.SeatNumber.Should().Be(seatCommand.SeatNumber);
        response?.FlightId.Should().Be(seatCommand.FlightId);
    }
}
