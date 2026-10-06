namespace Identity.Identity.Models;

using System;
using Griffin.Core.Model;
using Microsoft.AspNetCore.Identity;

public class RoleClaim : IdentityRoleClaim<Guid>, IVersion
{
    public long Version { get; set; }
}