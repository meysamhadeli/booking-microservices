using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase;
using FluentAssertions;
using Identity.Api;
using Identity.Data;
using Integration.Test.Fakes;
using MassTransit;
using MassTransit.Testing;
using Xunit;

namespace Integration.Test.Identity.Features;

public class RegisterNewUserTests : IntegrationTestBase<Program, IdentityContext>
{
    public RegisterNewUserTests(IntegrationTestFixture<Program, IdentityContext> integrationTestFixture) : base(
        integrationTestFixture)
    {
    }

    [Fact]
    public async Task should_create_new_user_to_db_and_publish_message_to_broker()
    {
        // Arrange
        var command = new FakeRegisterNewUserCommand().Generate();

        // Act
        var response = await Fixture.SendAsync(command);

        // Assert
        response?.Should().NotBeNull();
        response?.Username.Should().Be(command.Username);

        await Fixture.WaitForPublishing<UserCreated>();
    }
}
