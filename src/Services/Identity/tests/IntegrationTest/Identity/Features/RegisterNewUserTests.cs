using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using FluentAssertions;
using Integration.Test.Fakes;
using MassTransit;
using MassTransit.Testing;
using Xunit;

namespace Integration.Test.Identity.Features;

[Collection(nameof(TestFixture))]
public class RegisterNewUserTests
{
    private readonly TestFixture _fixture;
    private readonly ITestHarness _testHarness;

    public RegisterNewUserTests(TestFixture fixture)
    {
        _fixture = fixture;
        _testHarness = _fixture.TestHarness;
    }

    [Fact]
    public async Task should_create_new_user_to_db_and_publish_message_to_broker()
    {
        // Arrange
        var command = new FakeRegisterNewUserCommand().Generate();

        // Act
        var response = await _fixture.SendAsync(command);

        // Assert
        response?.Should().NotBeNull();
        response?.Username.Should().Be(command.Username);
        (await _testHarness.Published.Any<Fault<UserCreated>>()).Should().BeFalse();
        (await _testHarness.Published.Any<UserCreated>()).Should().BeTrue();
    }
}
