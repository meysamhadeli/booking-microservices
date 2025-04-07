using BookingMonolith.Identity.Identities.Features.RegisteringNewUser.V1;
using Mapster;

namespace BookingMonolith.Identity.Identities.Features;

public class IdentityMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterNewUserRequestDto, RegisterNewUser>()
            .ConstructUsing(x => new RegisterNewUser(x.FirstName, x.LastName, x.Username, x.Email,
                x.Password, x.ConfirmPassword, x.PassportNumber));
    }
}
