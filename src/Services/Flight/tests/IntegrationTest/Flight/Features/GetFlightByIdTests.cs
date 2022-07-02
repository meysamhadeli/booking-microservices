using System.Threading.Tasks;
using BuildingBlocks.Contracts.Grpc;
using Flight.Flights.Features.GetFlightById;
using FluentAssertions;
using Grpc.Net.Client;
using Integration.Test.Fakes;
using MagicOnion.Client;
using Xunit;

namespace Integration.Test.Flight.Features;

public class GetFlightByIdTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;
    private readonly GrpcChannel _channel;

    public GetFlightByIdTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _channel = fixture.Channel;
    }

    [Fact]
    public async Task should_retrive_a_flight_by_id_currectly()
    {
        // Arrange
        var command = new FakeCreateFlightCommand().Generate();
        var flightEntity = FakeFlightCreated.Generate(command);
        await _fixture.InsertAsync(flightEntity);

        var query = new GetFlightByIdQuery(flightEntity.Id);

        // Act
        var response = await _fixture.SendAsync(query);

        // Assert
        response.Should().NotBeNull();
        response?.Id.Should().Be(flightEntity.Id);
    }

    [Fact]
    public async Task should_retrive_a_flight_by_id_from_grpc_service()
    {
        // Arrange
        var command = new FakeCreateFlightCommand().Generate();
        var flightEntity = FakeFlightCreated.Generate(command);
        await _fixture.InsertAsync(flightEntity);

        var flightGrpcClient = MagicOnionClient.Create<IFlightGrpcService>(_channel);

        // Act
        var response = await flightGrpcClient.GetById(flightEntity.Id);

        // Assert
        response?.Should().NotBeNull();
        response?.Id.Should().Be(flightEntity.Id);
    }
}
