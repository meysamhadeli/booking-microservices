using AutoBogus;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.IdsGenerator;

namespace Integration.Test.Fakes;

public class FakeUserCreated : AutoFaker<UserCreated>
{
    public FakeUserCreated()
    {
        RuleFor(r => r.Id,  _ => SnowflakeIdGenerator.NewId());
        RuleFor(r => r.Name, _ => "Sam");
        RuleFor(r => r.PassportNumber, _ => "123456789");
    }
}
