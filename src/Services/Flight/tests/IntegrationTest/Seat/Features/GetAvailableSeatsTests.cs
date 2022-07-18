using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.Grpc;
using BuildingBlocks.TestBase;
using Flight.Data;
using Flight.Flights.Features.CreateFlight.Reads;
using Flight.Seats.Features.CreateSeat.Reads;
using FluentAssertions;
using Grpc.Net.Client;
using Integration.Test.Fakes;
using MagicOnion.Client;
using MassTransit.Testing;
using Xunit;

namespace Integration.Test.Seat.Features;

public class GetAvailableSeatsTests : IntegrationTestBase<Program, FlightDbContext, FlightReadDbContext>
{
    private readonly GrpcChannel _channel;

    public GetAvailableSeatsTests(IntegrationTestFixture<Program, FlightDbContext, FlightReadDbContext> integrationTestFixture) : base(integrationTestFixture)
    {
        _channel = Fixture.Channel;
    }

    [Fact]
    public async Task should_return_available_seats_from_grpc_service()
    {
        // Arrange
        var flightCommand = new FakeCreateFlightCommand().Generate();

        await Fixture.SendAsync(flightCommand);

        await Fixture.ShouldProcessedPersistInternalCommand<CreateFlightMongoCommand>();

        var seatCommand = new FakeCreateSeatCommand(flightCommand.Id).Generate();

        await Fixture.SendAsync(seatCommand);

        await Fixture.ShouldProcessedPersistInternalCommand<CreateSeatMongoCommand>();

        var flightGrpcClient = MagicOnionClient.Create<IFlightGrpcService>(_channel);

        // Act
        var response = await flightGrpcClient.GetAvailableSeats(flightCommand.Id);

        // Assert
        response?.Should().NotBeNull();
        response?.Count().Should().BeGreaterOrEqualTo(1);
    }
}
