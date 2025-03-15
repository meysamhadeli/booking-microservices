namespace Identity.Identity.Exceptions;

using BuildingBlocks.Exception;

public class RegisterIdentityUserException : AppException
{
    public RegisterIdentityUserException(string error) : base(error)
    {
    }
}
