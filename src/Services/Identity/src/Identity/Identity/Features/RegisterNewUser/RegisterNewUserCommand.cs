using Identity.Identity.Dtos;
using MediatR;

namespace Identity.Identity.Features.RegisterNewUser;

public record RegisterNewUserCommand(string FirstName, string LastName, string Username, string Email, string Password, string ConfirmPassword, string PassportNumber) : IRequest<RegisterNewUserResponseDto>;
