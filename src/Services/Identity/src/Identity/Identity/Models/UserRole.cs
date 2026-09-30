namespace Identity.Identity.Models;

using System;
using Woo.Core.Model;
using Microsoft.AspNetCore.Identity;

public class UserRole : IdentityUserRole<Guid>, IVersion
{
    public long Version { get; set; }
}