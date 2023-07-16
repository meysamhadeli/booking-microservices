using System.Threading.Tasks;
using BuildingBlocks.TestBase;
using FluentAssertions;
using Integration.Test.Fakes;
using Passenger;
using Passenger.Api;
using Passenger.Data;
using Xunit;

namespace Integration.Test.Passenger.Features;

using global::Passenger.Passengers.Features.GettingPassengerById.V1;
using Humanizer;
using Thrift.Protocol;

public class GetPassengerByIdTests : PassengerIntegrationTestBase
{
    public GetPassengerByIdTests(
        TestFixture<Program, PassengerDbContext, PassengerReadDbContext> integrationTestFactory) : base(integrationTestFactory)
    {
    }

    [Fact]
    public async Task should_retrive_a_passenger_by_id_currectly()
    {
        // Arrange
        var command = new FakeCompleteRegisterPassengerMongoCommand().Generate();

        await Fixture.SendAsync(command);

        var query = new GetPassengerById(command.Id);

        // Act
        var response = await Fixture.SendAsync(query);

        // Assert
        response.Should().NotBeNull();
        response?.PassengerDto?.Id.Should().Be(command.Id);
    }

    [Fact]
    public async Task should_retrive_a_passenger_by_id_from_grpc_service()
    {
        // Arrange
        var command = new FakeCompleteRegisterPassengerMongoCommand().Generate();

        await Fixture.SendAsync(command);

        var passengerGrpcClient = new PassengerGrpcService.PassengerGrpcServiceClient(Fixture.Channel);

        // Act
        var response = await passengerGrpcClient.GetByIdAsync(new GetByIdRequest {Id = command.Id.ToString()});

        // Assert
        response?.Should().NotBeNull();
        response?.PassengerDto?.Id.Should().Be(command.Id.ToString());
    }
}
