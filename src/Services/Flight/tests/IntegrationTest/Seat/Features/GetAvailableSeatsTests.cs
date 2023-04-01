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

public class GetAvailableSeatsTests : FlightIntegrationTestBase
{
    public GetAvailableSeatsTests(
        TestFixture<Program, FlightDbContext, FlightReadDbContext> integrationTestFactory) : base(integrationTestFactory)
    {
    }

    [Fact]
    public async Task should_return_available_seats_from_grpc_service()
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
        var response = await flightGrpcClient.GetAvailableSeatsAsync(new GetAvailableSeatsRequest{FlightId = flightCommand.Id.ToString()});

        // Assert
        response?.Should().NotBeNull();
        response?.SeatDtos?.Count.Should().BeGreaterOrEqualTo(1);
    }
}
