using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using FluentAssertions;
using Integration.Test.Fakes;
using MassTransit.Testing;
using Xunit;

namespace Integration.Test.Passenger.Features;

public class CompleteRegisterPassengerTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;
    private readonly ITestHarness _testHarness;

    public CompleteRegisterPassengerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _testHarness = _fixture.TestHarness;
    }

    [Fact]
    public async Task should_complete_register_passenger_and_update_to_db()
    {
        // Arrange
        var userCreated = new FakeUserCreated().Generate();
        await _testHarness.Bus.Publish(userCreated);
        await _testHarness.Consumed.Any<UserCreated>();
        await _fixture.InsertAsync(FakePassengerCreated.Generate(userCreated));

        var command = new FakeCompleteRegisterPassengerCommand(userCreated.PassportNumber).Generate();

        // Act
        var response = await _fixture.SendAsync(command);

        // Assert
        response.Should().NotBeNull();
        response?.Name.Should().Be(userCreated.Name);
        response?.PassportNumber.Should().Be(command.PassportNumber);
        response?.PassengerType.Should().Be(command.PassengerType);
        response?.Age.Should().Be(command.Age);
    }
}
