using AutoBogus;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.IdsGenerator;

namespace Integration.Test.Fakes;

public class FakeUserCreated : AutoFaker<UserCreated>
{
    public FakeUserCreated()
    {
        RuleFor(r => r.Id,  _ => _.Random.Number(50, 100000));
        RuleFor(r => r.Name, _ => "Meysam");
        RuleFor(r => r.PassportNumber, _ => "1299878");
    }
}
