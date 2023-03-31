using System.Threading.Tasks;
using BuildingBlocks.TestBase;
using FluentAssertions;
using Integration.Test.Fakes;
using Passenger;
using Passenger.Api;
using Passenger.Data;
using Xunit;

namespace Integration.Test.Passenger.Features;

using global::Passenger.Passengers.Features.GettingPassengerById.Queries.V1;
using Thrift.Protocol;

public class GetPassengerByIdTests : PassengerIntegrationTestBase
{
    public GetPassengerByIdTests(
        TestWriteFixture<Program, PassengerDbContext> integrationTestFactory) : base(integrationTestFactory)
    {
    }

    [Fact]
    public async Task should_retrive_a_passenger_by_id_currectly()
    {
        // Arrange
        var userCreated = new FakeUserCreated().Generate();

        var passengerEntity = FakePassengerCreated.Generate(userCreated);
        await Fixture.InsertAsync(passengerEntity);

        var query = new GetPassengerById(passengerEntity.Id);

        // Act
        var response = await Fixture.SendAsync(query);

        // Assert
        response.Should().NotBeNull();
        response?.PassengerDto?.Id.Should().Be(passengerEntity.Id);
    }

    [Fact]
    public async Task should_retrive_a_passenger_by_id_from_grpc_service()
    {
        // Arrange
        var userCreated = new FakeUserCreated().Generate();

        var passengerEntity = FakePassengerCreated.Generate(userCreated);
        await Fixture.InsertAsync(passengerEntity);

        var passengerGrpcClient = new PassengerGrpcService.PassengerGrpcServiceClient(Fixture.Channel);

        // Act
        var response = await passengerGrpcClient.GetByIdAsync(new GetByIdRequest {Id = passengerEntity.Id.ToString()});

        // Assert
        response?.Should().NotBeNull();
        response?.PassengerDto?.Id.Should().Be(passengerEntity.Id.ToString());
    }
}
