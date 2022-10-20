using BuildingBlocks.Exception;

namespace Identity.Identity.Features.RegisterNewUser.Exceptions;

public class RegisterIdentityUserException : AppException
{
    public RegisterIdentityUserException(string error) : base(error)
    {
    }
}
