using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase;
using FluentAssertions;
using Grpc.Net.Client;
using Integration.Test.Fakes;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Passenger;
using Passenger.Api;
using Passenger.Data;
using Passenger.Passengers.Features.GetPassengerById;
using Passenger.Passengers.Features.GetPassengerById.Queries.V1;
using Xunit;

namespace Integration.Test.Passenger.Features;

public class GetPassengerByIdTests : IntegrationTestBase<Program, PassengerDbContext>
{
    private readonly ITestHarness _testHarness;
    private readonly GrpcChannel _channel;

    public GetPassengerByIdTests(IntegrationTestFixture<Program, PassengerDbContext> integrationTestFixture) : base(
        integrationTestFixture)
    {
        _channel = Fixture.Channel;
        _testHarness = Fixture.TestHarness;
    }

    protected override void RegisterTestsServices(IServiceCollection services)
    {
    }


    [Fact]
    public async Task should_retrive_a_passenger_by_id_currectly()
    {
        // Arrange
        var userCreated = new FakeUserCreated().Generate();
        await _testHarness.Bus.Publish(userCreated);
        await _testHarness.Consumed.Any<UserCreated>();
        var passengerEntity = FakePassengerCreated.Generate(userCreated);
        await Fixture.InsertAsync(passengerEntity);

        var query = new GetPassengerQueryById(passengerEntity.Id);

        // Act
        var response = await Fixture.SendAsync(query);

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
        await Fixture.InsertAsync(passengerEntity);

        var passengerGrpcClient = new PassengerGrpcService.PassengerGrpcServiceClient(_channel);

        // Act
        var response = await passengerGrpcClient.GetByIdAsync(new GetByIdRequest {Id = passengerEntity.Id});

        // Assert
        response?.Should().NotBeNull();
        response?.Id.Should().Be(passengerEntity.Id);
    }
}
