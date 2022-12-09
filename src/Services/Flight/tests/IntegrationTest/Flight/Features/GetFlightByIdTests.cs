using System.Threading.Tasks;
using BuildingBlocks.TestBase.IntegrationTest;
using Flight;
using Flight.Api;
using Flight.Data;
using Flight.Flights.Features.CreateFlight.Commands.V1.Reads;
using Flight.Flights.Features.GetFlightById.Queries.V1;
using FluentAssertions;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Flight.Features;

public class GetFlightByIdTests : FlightIntegrationTestBase
{
    public GetFlightByIdTests(
        IntegrationTestFactory<Program, FlightDbContext, FlightReadDbContext> integrationTestFactory) : base(integrationTestFactory)
    {
    }

    [Fact]
    public async Task should_retrive_a_flight_by_id_currectly()
    {
        //Arrange
        var command = new FakeCreateFlightCommand().Generate();
        await Fixture.SendAsync(command);

        (await Fixture.ShouldProcessedPersistInternalCommand<CreateFlightMongoCommand>()).Should().Be(true);

        var query = new GetFlightByIdQuery(command.Id);

        // Act
        var response = await Fixture.SendAsync(query);

        // Assert
        response.Should().NotBeNull();
        response?.Id.Should().Be(command.Id);
    }

    [Fact]
    public async Task should_retrive_a_flight_by_id_from_grpc_service()
    {
        //Arrange
        var command = new FakeCreateFlightCommand().Generate();
        await Fixture.SendAsync(command);

        (await Fixture.ShouldProcessedPersistInternalCommand<CreateFlightMongoCommand>()).Should().Be(true);

        var flightGrpcClient = new FlightGrpcService.FlightGrpcServiceClient(Fixture.Channel);

        // Act
        var response = await flightGrpcClient.GetByIdAsync(new GetByIdRequest {Id = command.Id});

        // Assert
        response?.Should().NotBeNull();
        response?.Id.Should().Be(command.Id);
    }
}
