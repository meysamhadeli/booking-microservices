namespace Identity.Identity.Models;

using BuildingBlocks.Core.Model;
using Microsoft.AspNetCore.Identity;

public class UserClaim: IdentityUserClaim<long>, IVersion
{
    public long Version { get; set; }
}
