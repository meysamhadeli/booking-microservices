using System.Threading.Tasks;
using BuildingBlocks.Contracts.Grpc;
using BuildingBlocks.TestBase;
using Flight.Data;
using Flight.Flights.Features.CreateFlight.Reads;
using Flight.Flights.Features.GetFlightById;
using FluentAssertions;
using Grpc.Net.Client;
using Integration.Test.Fakes;
using MagicOnion.Client;
using Xunit;

namespace Integration.Test.Flight.Features;

public class GetFlightByIdTests : IntegrationTestBase<Program, FlightDbContext, FlightReadDbContext>
{
    private readonly GrpcChannel _channel;

    public GetFlightByIdTests(IntegrationTestFixture<Program, FlightDbContext, FlightReadDbContext> integrationTestFixture) : base(integrationTestFixture)
    {
        _channel = Fixture.Channel;
    }

    [Fact]
    public async Task should_retrive_a_flight_by_id_currectly()
    {
        //Arrange
        var command = new FakeCreateFlightCommand().Generate();
        await Fixture.SendAsync(command);

        await Fixture.ShouldProcessedPersistInternalCommand<CreateFlightMongoCommand>();

        var query = new GetFlightByIdQuery(command.Id);

        // Act
        var response = await Fixture.SendAsync(query);

        // Assert
        response.Should().NotBeNull();
        response?.FlightId.Should().Be(command.Id);
    }

    [Fact]
    public async Task should_retrive_a_flight_by_id_from_grpc_service()
    {
        //Arrange
        var command = new FakeCreateFlightCommand().Generate();
        await Fixture.SendAsync(command);

        await Fixture.ShouldProcessedPersistInternalCommand<CreateFlightMongoCommand>();

        var flightGrpcClient = MagicOnionClient.Create<IFlightGrpcService>(_channel);

        // Act
        var response = await flightGrpcClient.GetById(command.Id);

        // Assert
        response?.Should().NotBeNull();
        response?.FlightId.Should().Be(command.Id);
    }
}
