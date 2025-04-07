using BuildingBlocks.Core.Model;
using Microsoft.AspNetCore.Identity;

namespace BookingMonolith.Identity.Identities.Models;

public class UserRole : IdentityUserRole<Guid>, IVersion
{
    public long Version { get; set; }
}
