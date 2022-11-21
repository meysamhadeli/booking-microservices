namespace Identity.Identity.Features.RegisterNewUser.Dtos.V1;

public record RegisterNewUserRequestDto(string FirstName, string LastName, string Username, string Email,
    string Password, string ConfirmPassword, string PassportNumber);
