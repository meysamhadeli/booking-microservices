using System.Threading.Tasks;
using BuildingBlocks.TestBase;
using FluentAssertions;
using Integration.Test.Fakes;
using Passenger;
using Passenger.Api;
using Passenger.Data;
using Passenger.Passengers.Features.GetPassengerById.Queries.V1;
using Xunit;

namespace Integration.Test.Passenger.Features;

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

        var passengerEntity = FakePassengerCreated.Generate(userCreated);
        await Fixture.InsertAsync(passengerEntity);

        var passengerGrpcClient = new PassengerGrpcService.PassengerGrpcServiceClient(Fixture.Channel);

        // Act
        var response = await passengerGrpcClient.GetByIdAsync(new GetByIdRequest {Id = passengerEntity.Id});

        // Assert
        response?.Should().NotBeNull();
        response?.Id.Should().Be(passengerEntity.Id);
    }
}
