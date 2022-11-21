using Identity.Identity.Features.RegisterNewUser.Commands.V1;
using Identity.Identity.Features.RegisterNewUser.Dtos.V1;
using Mapster;

namespace Identity.Identity.Features;

public class IdentityMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterNewUserRequestDto, RegisterNewUserCommand>()
            .ConstructUsing(x => new RegisterNewUserCommand(x.FirstName, x.LastName, x.Username, x.Email,
                x.Password, x.ConfirmPassword, x.PassportNumber));
    }
}
