using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using FluentAssertions;
using Integration.Test.Fakes;
using MassTransit.Testing;
using Passenger.Passengers.Features.GetPassengerById;
using Xunit;

namespace Integration.Test.Passenger.Features;

[Collection(nameof(TestFixture))]
public class GetPassengerByIdTests
{
    private readonly TestFixture _fixture;
    private readonly ITestHarness _testHarness;

    public GetPassengerByIdTests(TestFixture fixture)
    {
        _fixture = fixture;
        _testHarness = _fixture.TestHarness;
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
}
