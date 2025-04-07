using BuildingBlocks.Core.Model;
using Microsoft.AspNetCore.Identity;

namespace BookingMonolith.Identity.Identities.Models;

public class Role : IdentityRole<Guid>, IVersion
{
    public long Version { get; set; }
}
