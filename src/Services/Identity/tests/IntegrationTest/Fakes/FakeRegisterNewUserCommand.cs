using AutoBogus;
using Identity.Identity.Features.RegisterNewUser;
using Identity.Identity.Features.RegisterNewUser.Commands.V1;

namespace Integration.Test.Fakes;

public class FakeRegisterNewUserCommand : AutoFaker<RegisterNewUserCommand>
{
    public FakeRegisterNewUserCommand()
    {
        RuleFor(r => r.Username, _ => "TestUser");
        RuleFor(r => r.Password, _ => "Password@123");
        RuleFor(r => r.ConfirmPassword, _ => "Password@123");
        RuleFor(r => r.Email, _ => "test@test.com");
    }
}
