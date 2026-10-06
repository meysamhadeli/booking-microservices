using Griffin.TestBase;
using Griffin.Core.EventBus.Messages;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Passenger.Data;
using Xunit;

namespace Integration.Test.Passenger.Features;

public class RegisterNewUserTests : PassengerIntegrationTestBase
{
    public RegisterNewUserTests(
        TestFixture<Program, PassengerDbContext, PassengerReadDbContext> integrationTestFactory)
        : base(integrationTestFactory) { }

    [Fact]
    public async Task should_create_passenger_when_user_created_event_is_published()
    {
        var userCreated = new UserCreated(Guid.CreateVersion7(), "Sam", "123456789");

        await Fixture.Publish(userCreated);

        var passenger = await Fixture.ExecuteDbContextAsync(db =>
            db.Passengers.FirstOrDefaultAsync(x => x.PassportNumber.Value == userCreated.PassportNumber));

        passenger.Should().NotBeNull();
        passenger!.Id.Value.Should().Be(userCreated.Id);
        passenger.Name.Value.Should().Be(userCreated.Name);
        passenger.PassportNumber.Value.Should().Be(userCreated.PassportNumber);
    }
}