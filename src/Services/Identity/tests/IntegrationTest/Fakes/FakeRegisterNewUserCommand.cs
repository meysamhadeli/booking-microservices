using AutoBogus;
using Identity.Identity.Features.RegisterNewUser;

namespace Integration.Test.Fakes;

public class FakeRegisterNewUserCommand : AutoFaker<RegisterNewUserCommand>
{
    public FakeRegisterNewUserCommand()
    {
        RuleFor(r => r.Password, _ => "Password@123");
        RuleFor(r => r.ConfirmPassword, _ => "Password@123");
        RuleFor(r => r.Email, _ => "test@test.com");
    }
}
