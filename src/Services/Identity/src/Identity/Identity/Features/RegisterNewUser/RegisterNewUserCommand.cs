using BuildingBlocks.Core.CQRS;
using Identity.Identity.Dtos;

namespace Identity.Identity.Features.RegisterNewUser;

public record RegisterNewUserCommand(string FirstName, string LastName, string Username, string Email,
    string Password, string ConfirmPassword, string PassportNumber) : ICommand<RegisterNewUserResponseDto>;
