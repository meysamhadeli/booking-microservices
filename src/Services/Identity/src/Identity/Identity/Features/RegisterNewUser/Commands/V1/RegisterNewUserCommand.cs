using BuildingBlocks.Core.CQRS;
using Identity.Identity.Dtos;

namespace Identity.Identity.Features.RegisterNewUser.Commands.V1;

public record RegisterNewUserCommand(string FirstName, string LastName, string Username, string Email,
    string Password, string ConfirmPassword, string PassportNumber) : ICommand<RegisterNewUserResponseDto>;
