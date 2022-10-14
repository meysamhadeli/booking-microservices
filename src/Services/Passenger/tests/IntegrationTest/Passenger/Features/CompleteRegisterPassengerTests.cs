using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase;
using FluentAssertions;
using Integration.Test.Fakes;
using MassTransit.Testing;
using Passenger.Data;
using Xunit;

namespace Integration.Test.Passenger.Features;

public class CompleteRegisterPassengerTests : IntegrationTestBase<Program, PassengerDbContext>
{
    private readonly ITestHarness _testHarness;

    public CompleteRegisterPassengerTests(IntegrationTestFixture<Program, PassengerDbContext> integrationTestFixture) :
        base(integrationTestFixture)
    {
        _testHarness = Fixture.TestHarness;
    }

    [Fact]
    public async Task should_complete_register_passenger_and_update_to_db()
    {
        // Arrange
        var userCreated = new FakeUserCreated().Generate();
        await _testHarness.Bus.Publish(userCreated);
        await _testHarness.Consumed.Any<UserCreated>();
        await Fixture.InsertAsync(FakePassengerCreated.Generate(userCreated));

        var command = new FakeCompleteRegisterPassengerCommand(userCreated.PassportNumber).Generate();

        // Act
        var response = await Fixture.SendAsync(command);

        // Assert
        response.Should().NotBeNull();
        response?.Name.Should().Be(userCreated.Name);
        response?.PassportNumber.Should().Be(command.PassportNumber);
        response?.PassengerType.ToString().Should().Be(command.PassengerType.ToString());
        response?.Age.Should().Be(command.Age);
    }
}
