using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Domain;
using Identity.Identity.Dtos;
using Identity.Identity.Exceptions;
using Identity.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Identity.Features.RegisterNewUser;

public class RegisterNewUserCommandHandler : IRequestHandler<RegisterNewUserCommand, RegisterNewUserResponseDto>
{
    private readonly IBusPublisher _busPublisher;
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterNewUserCommandHandler(UserManager<ApplicationUser> userManager,
        IBusPublisher busPublisher)
    {
        _userManager = userManager;
        _busPublisher = busPublisher;
    }

    public async Task<RegisterNewUserResponseDto> Handle(RegisterNewUserCommand command,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var applicationUser = new ApplicationUser
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            UserName = command.Username,
            Email = command.Email,
            PasswordHash = command.Password,
            PassPortNumber = command.PassportNumber
        };

        var identityResult = await _userManager.CreateAsync(applicationUser, command.Password);
        var roleResult = await _userManager.AddToRoleAsync(applicationUser, Constants.Constants.Role.User);

        if (identityResult.Succeeded == false)
            throw new RegisterIdentityUserException(string.Join(',', identityResult.Errors.Select(e => e.Description)));

        if (roleResult.Succeeded == false)
            throw new RegisterIdentityUserException(string.Join(',', roleResult.Errors.Select(e => e.Description)));

        await _busPublisher.SendAsync(new UserCreated(applicationUser.Id, applicationUser.FirstName + " " + applicationUser.LastName,
                applicationUser.PassPortNumber), cancellationToken);

        return new RegisterNewUserResponseDto
        {
            Id = applicationUser.Id,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            Username = applicationUser.UserName,
            PassportNumber = applicationUser.PassPortNumber
        };
    }
}
