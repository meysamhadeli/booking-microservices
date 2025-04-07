using BuildingBlocks.Core.Model;
using Microsoft.AspNetCore.Identity;

namespace BookingMonolith.Identity.Identities.Models;

public class User : IdentityUser<Guid>, IVersion
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string PassPortNumber { get; init; }
    public long Version { get; set; }
}
