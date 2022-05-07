using BuildingBlocks.Exception;

namespace Identity.Identity.Exceptions;

public class RegisterIdentityUserException : AppException
{
    public RegisterIdentityUserException(string error) : base(error)
    {
    }
}