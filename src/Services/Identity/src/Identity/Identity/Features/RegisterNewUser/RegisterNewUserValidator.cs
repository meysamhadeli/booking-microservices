using FluentValidation;

namespace Identity.Identity.Features.RegisterNewUser;

public class RegisterNewUserValidator : AbstractValidator<RegisterNewUserCommand>
{
    public RegisterNewUserValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Password).NotEmpty().WithMessage("Please enter the password");
        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Please enter the confirmation password");

        RuleFor(x => x).Custom((x, context) =>
        {
            if (x.Password != x.ConfirmPassword)
            {
                context.AddFailure(nameof(x.Password), "Passwords should match");
            }
        });
        
        RuleFor(x => x.Username).NotEmpty().WithMessage("Please enter the username");
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Please enter the first name");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Please enter the last name");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Please enter the last email")
            .EmailAddress().WithMessage("A valid email is required");
    }
}
