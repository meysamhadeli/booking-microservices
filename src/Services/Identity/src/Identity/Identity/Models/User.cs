using Microsoft.AspNetCore.Identity;

namespace Identity.Identity.Models;

using System;
using BuildingBlocks.Core.Model;

public class User : IdentityUser<Guid>, IVersion
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string PassPortNumber { get; init; }
    public long Version { get; set; }
}
