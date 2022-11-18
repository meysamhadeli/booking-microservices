using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.TestBase;
using FluentAssertions;
using Grpc.Net.Client;
using Identity.Api;
using Identity.Data;
using Integration.Test.Fakes;
using MassTransit;
using MassTransit.Testing;
using Xunit;

namespace Integration.Test.Identity.Features;

public class RegisterNewUserTests : IntegrationTestBase<Program, IdentityContext>
{
    private readonly ITestHarness _testHarness;

    public RegisterNewUserTests(IntegrationTestFactory<Program, IdentityContext> integrationTestFixture) : base(integrationTestFixture)
    {
        _testHarness = Fixture.TestHarness;
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
        (await _testHarness.Published.Any<Fault<UserCreated>>()).Should().BeFalse();
        (await _testHarness.Published.Any<UserCreated>()).Should().BeTrue();
    }
}
