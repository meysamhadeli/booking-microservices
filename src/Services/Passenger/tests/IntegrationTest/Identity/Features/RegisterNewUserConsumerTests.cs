using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using Integration.Test.Fakes;
using MassTransit.Testing;
using Xunit;

namespace Integration.Test.Identity.Features;

[Collection(nameof(TestFixture))]
public class RegisterNewUserConsumerTests
{
    private readonly TestFixture _fixture;
    private readonly ITestHarness _testHarness;

    public RegisterNewUserConsumerTests(TestFixture fixture)
    {
        _fixture = fixture;
        _testHarness = _fixture.TestHarness;
    }

    [Fact]
    public async Task should_register_new_user_consume_handler_consume_user_created_message()
    {
        // // Arrange
        var message = new FakeUserCreated().Generate();

        // // Act
        await _testHarness.Bus.Publish(message);

        // // Assert
        await _testHarness.Consumed.Any<UserCreated>();
    }
}
