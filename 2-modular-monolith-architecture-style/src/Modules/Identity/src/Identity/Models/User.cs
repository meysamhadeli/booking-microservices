using Microsoft.AspNetCore.Identity;

namespace Identity.Identity.Models;

using System;
using BuildingBlocks.Core.Model;

public class User : IdentityUser<Guid>, IVersion
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string PassPortNumber { get; init; }
    public long Version { get; set; }
}
