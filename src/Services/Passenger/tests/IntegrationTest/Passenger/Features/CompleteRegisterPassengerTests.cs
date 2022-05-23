using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.IdsGenerator;
using FluentAssertions;
using Integration.Test.Fakes;
using MassTransit;
using MassTransit.Testing;
using Xunit;

namespace Integration.Test.Passenger.Features;

[Collection(nameof(TestFixture))]
public class CompleteRegisterPassengerTests
{
    private readonly TestFixture _fixture;
    private readonly ITestHarness _testHarness;

    public CompleteRegisterPassengerTests(TestFixture fixture)
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
