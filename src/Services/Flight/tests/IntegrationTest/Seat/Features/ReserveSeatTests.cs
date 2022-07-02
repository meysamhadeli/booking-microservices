using System.Threading.Tasks;
using BuildingBlocks.Contracts.Grpc;
using FluentAssertions;
using Grpc.Net.Client;
using Integration.Test.Fakes;
using MagicOnion.Client;
using Xunit;

namespace Integration.Test.Seat.Features;
public class ReserveSeatTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;
    private readonly GrpcChannel _channel;

    public ReserveSeatTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _channel = fixture.Channel;
    }

    [Fact]
    public async Task should_return_valid_reserve_seat_from_grpc_service()
    {
        // Arrange
        var flightCommand = new FakeCreateFlightCommand().Generate();
        var flightEntity = FakeFlightCreated.Generate(flightCommand);

        await _fixture.InsertAsync(flightEntity);

        var seatCommand = new FakeCreateSeatCommand(flightEntity.Id).Generate();
        var seatEntity = FakeSeatCreated.Generate(seatCommand);

        await _fixture.InsertAsync(seatEntity);

        var flightGrpcClient = MagicOnionClient.Create<IFlightGrpcService>(_channel);

        // Act
        var response = await flightGrpcClient.ReserveSeat(new ReserveSeatRequestDto{ FlightId = seatEntity.FlightId, SeatNumber = seatEntity.SeatNumber });

        // Assert
        response?.Should().NotBeNull();
        response?.SeatNumber.Should().Be(seatEntity.SeatNumber);
        response?.FlightId.Should().Be(seatEntity.FlightId);
    }
}
