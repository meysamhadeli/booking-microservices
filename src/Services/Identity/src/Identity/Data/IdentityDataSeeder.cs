using System;
using System.Threading.Tasks;
using BuildingBlocks.EFCore;
using Identity.Identity.Constants;
using Identity.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Data;

public class IdentityDataSeeder : IDataSeeder
{
    private readonly RoleManager<IdentityRole<long>> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityDataSeeder(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<long>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAllAsync()
    {
        await SeedRoles();
        await SeedUsers();
    }

    private async Task SeedRoles()
    {
        if (await _roleManager.RoleExistsAsync(Constants.Role.Admin) == false)
            await _roleManager.CreateAsync(new(Constants.Role.Admin));

        if (await _roleManager.RoleExistsAsync(Constants.Role.User) == false)
            await _roleManager.CreateAsync(new(Constants.Role.User));
    }

    private async Task SeedUsers()
    {
        if (await _userManager.FindByNameAsync("meysamh") == null)
        {
            var user = new ApplicationUser
            {
                FirstName = "Meysam",
                LastName = "Hadeli",
                UserName = "meysamh",
                Email = "meysam@test.com",
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, "Admin@123456");

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, Constants.Role.Admin);
        }

        if (await _userManager.FindByNameAsync("meysamh2") == null)
        {
            var user = new ApplicationUser
            {
                FirstName = "Meysam",
                LastName = "Hadeli",
                UserName = "meysamh2",
                Email = "meysam2@test.com",
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, "User@123456");

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, Constants.Role.User);
        }
    }
}
