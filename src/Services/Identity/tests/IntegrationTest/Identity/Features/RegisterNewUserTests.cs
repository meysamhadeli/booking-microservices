using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase;
using FluentAssertions;
using Identity.Api;
using Identity.Data;
using Integration.Test.Fakes;
using Xunit;

namespace Integration.Test.Identity.Features;

public class RegisterNewUserTests : IdentityIntegrationTestBase
{
    public RegisterNewUserTests(
        TestWriteFixture<Program, IdentityContext> integrationTestFactory) : base(integrationTestFactory)
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

        (await Fixture.WaitForPublishing<UserCreated>()).Should().Be(true);
    }
}
