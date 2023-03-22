using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase;
using FluentAssertions;
using Integration.Test.Fakes;
using Passenger.Api;
using Passenger.Data;
using Xunit;

namespace Integration.Test.Passenger.Features;

public class CompleteRegisterPassengerTests : PassengerIntegrationTestBase
{
    public CompleteRegisterPassengerTests(
        TestWriteFixture<Program, PassengerDbContext> integrationTestFactory) : base(integrationTestFactory)
    {
    }

    [Fact]
    public async Task should_complete_register_passenger_and_update_to_db()
    {
        // Arrange
        var userCreated = new FakeUserCreated().Generate();

        await Fixture.Publish(userCreated);
        await Fixture.WaitForPublishing<UserCreated>();
        await Fixture.WaitForConsuming<UserCreated>();

        var command = new FakeCompleteRegisterPassengerCommand(userCreated.PassportNumber).Generate();

        // Act
        var response = await Fixture.SendAsync(command);

        // Assert
        response.Should().NotBeNull();
        response?.PassengerDto?.Name.Should().Be(userCreated.Name);
        response?.PassengerDto?.PassportNumber.Should().Be(command.PassportNumber);
        response?.PassengerDto?.PassengerType.ToString().Should().Be(command.PassengerType.ToString());
        response?.PassengerDto?.Age.Should().Be(command.Age);
    }
}
