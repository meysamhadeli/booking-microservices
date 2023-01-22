using Microsoft.AspNetCore.Identity;

namespace Identity.Identity.Models;

using BuildingBlocks.Core.Model;

public class User : IdentityUser<long>, IVersion
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string PassPortNumber { get; init; }
    public long Version { get; set; }
}
