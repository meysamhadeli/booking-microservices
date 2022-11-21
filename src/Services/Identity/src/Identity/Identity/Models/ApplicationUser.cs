using Microsoft.AspNetCore.Identity;

namespace Identity.Identity.Models;

public class ApplicationUser : IdentityUser<long>
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string PassPortNumber { get; init; }
}
