using Mapster;

namespace Identity.Identity.Features;

using RegisteringNewUser.V1;

public class IdentityMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterNewUserRequestDto, RegisterNewUser>()
            .ConstructUsing(x => new RegisterNewUser(x.FirstName, x.LastName, x.Username, x.Email,
                x.Password, x.ConfirmPassword, x.PassportNumber));

        config.NewConfig<RegisterNewUserResult, RegisterNewUserResponseDto>()
            .ConstructUsing(x => new RegisterNewUserResponseDto(x.Id, x.FirstName, x.LastName, x.Username, x.PassportNumber));
    }
}
