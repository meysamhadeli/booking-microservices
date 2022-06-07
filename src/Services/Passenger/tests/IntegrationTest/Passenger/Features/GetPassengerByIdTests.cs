using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Contracts.Grpc;
using FluentAssertions;
using Grpc.Net.Client;
using Integration.Test.Fakes;
using MagicOnion.Client;
using MassTransit.Testing;
using Passenger.Passengers.Features.GetPassengerById;
using Xunit;

namespace Integration.Test.Passenger.Features;

[Collection(nameof(IntegrationTestFixture))]
public class GetPassengerByIdTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly ITestHarness _testHarness;
    private readonly GrpcChannel _channel;

    public GetPassengerByIdTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _testHarness = _fixture.TestHarness;
        _channel = _fixture.Channel;
    }

    [Fact]
    public async Task should_retrive_a_passenger_by_id_currectly()
    {
        // Arrange
        var userCreated = new FakeUserCreated().Generate();
        await _testHarness.Bus.Publish(userCreated);
        await _testHarness.Consumed.Any<UserCreated>();
        var passengerEntity = FakePassengerCreated.Generate(userCreated);
        await _fixture.InsertAsync(passengerEntity);

        var query = new GetPassengerQueryById(passengerEntity.Id);

        // Act
        var response = await _fixture.SendAsync(query);

        // Assert
        response.Should().NotBeNull();
        response?.Id.Should().Be(passengerEntity.Id);
    }

    [Fact]
    public async Task should_retrive_a_passenger_by_id_from_grpc_service()
    {
        // Arrange
        var userCreated = new FakeUserCreated().Generate();
        await _testHarness.Bus.Publish(userCreated);
        await _testHarness.Consumed.Any<UserCreated>();
        var passengerEntity = FakePassengerCreated.Generate(userCreated);
        await _fixture.InsertAsync(passengerEntity);

        var passengerGrpcClient = MagicOnionClient.Create<IPassengerGrpcService>(_channel);

        // Act
        var response = await passengerGrpcClient.GetById(passengerEntity.Id);

        // Assert
        response?.Should().NotBeNull();
        response?.Id.Should().Be(passengerEntity.Id);
    }
}
