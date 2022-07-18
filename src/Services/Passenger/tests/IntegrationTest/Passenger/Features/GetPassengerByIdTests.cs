using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Contracts.Grpc;
using BuildingBlocks.TestBase;
using FluentAssertions;
using Grpc.Net.Client;
using Integration.Test.Fakes;
using MagicOnion;
using MagicOnion.Client;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Passenger.Data;
using Passenger.Passengers.Features.GetPassengerById;
using Xunit;

namespace Integration.Test.Passenger.Features;

public class GetPassengerByIdTests : IntegrationTestBase<Program, PassengerDbContext>
{
    private readonly ITestHarness _testHarness;
    private readonly GrpcChannel _channel;

    public GetPassengerByIdTests(IntegrationTestFixture<Program, PassengerDbContext> integrationTestFixture) : base(integrationTestFixture)
    {
        _channel = Fixture.Channel;
        _testHarness = Fixture.TestHarness;
    }

    protected override void RegisterTestsServices(IServiceCollection services)
    {
        MockPassengerGrpcServices(services);
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

        var passengerGrpcClient = MagicOnionClient.Create<IPassengerGrpcService>(_channel);

        // Act
        var response = await passengerGrpcClient.GetById(passengerEntity.Id);

        // Assert
        response?.Should().NotBeNull();
        response?.Id.Should().Be(passengerEntity.Id);
    }

    private void MockPassengerGrpcServices(IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton(x =>
        {
            var mock = Substitute.For<IPassengerGrpcService>();
            mock.GetById(Arg.Any<long>())
                .Returns(new UnaryResult<PassengerResponseDto>(new FakePassengerResponseDto().Generate()));

            return mock;
        }));
    }
}
