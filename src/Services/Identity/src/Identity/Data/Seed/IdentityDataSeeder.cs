using System;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Core;
using BuildingBlocks.EFCore;
using Identity.Identity.Constants;
using Identity.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Data.Seed;

public class IdentityDataSeeder : IDataSeeder
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IEventDispatcher _eventDispatcher;

    public IdentityDataSeeder(UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IEventDispatcher eventDispatcher)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _eventDispatcher = eventDispatcher;
    }

    public async Task SeedAllAsync()
    {
        await SeedRoles();
        await SeedUsers();
    }

    private async Task SeedRoles()
    {
        if (await _roleManager.RoleExistsAsync(Constants.Role.Admin) == false)
            await _roleManager.CreateAsync(new Role {Name = Constants.Role.Admin});

        if (await _roleManager.RoleExistsAsync(Constants.Role.User) == false)
            await _roleManager.CreateAsync(new Role {Name = Constants.Role.User});
    }

    private async Task SeedUsers()
    {
        if (await _userManager.FindByNameAsync("samh") == null)
        {
            var user = new User
            {
                FirstName = "Sam",
                LastName = "H",
                UserName = "samh",
                PassPortNumber = "123456789",
                Email = "sam@test.com",
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, "Admin@123456");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Constants.Role.Admin);

                await _eventDispatcher.SendAsync(new UserCreated(user.Id, user.FirstName + " " + user.LastName, user.PassPortNumber));
            }
        }

        if (await _userManager.FindByNameAsync("meysamh2") == null)
        {
            var user = new User
            {
                FirstName = "Sam2",
                LastName = "H2",
                UserName = "samh2",
                PassPortNumber = "987654321",
                Email = "sam2@test.com",
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, "User@123456");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Constants.Role.User);

                await _eventDispatcher.SendAsync(new UserCreated(user.Id, user.FirstName + " " + user.LastName, user.PassPortNumber));
            }
        }
    }
}
