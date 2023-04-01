using System;
using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Core;
using BuildingBlocks.EFCore;
using Identity.Identity.Constants;
using Identity.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Data.Seed;

using System.Linq;

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
        {
            await _roleManager.CreateAsync(new Role {Name = Constants.Role.Admin});
        }

        if (await _roleManager.RoleExistsAsync(Constants.Role.User) == false)
        {
            await _roleManager.CreateAsync(new Role {Name = Constants.Role.User});
        }
    }

    private async Task SeedUsers()
    {
        if (await _userManager.FindByNameAsync("samh") == null)
        {
            var result = await _userManager.CreateAsync(InitialData.Users.First(), "Admin@123456");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(InitialData.Users.First(), Constants.Role.Admin);

                await _eventDispatcher.SendAsync(new UserCreated(InitialData.Users.First().Id, InitialData.Users.First().FirstName + " " + InitialData.Users.First().LastName, InitialData.Users.First().PassPortNumber));
            }
        }

        if (await _userManager.FindByNameAsync("meysamh2") == null)
        {
            var result = await _userManager.CreateAsync(InitialData.Users.Last(), "User@123456");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(InitialData.Users.Last(), Constants.Role.User);

                await _eventDispatcher.SendAsync(new UserCreated(InitialData.Users.Last().Id, InitialData.Users.Last().FirstName + " " + InitialData.Users.Last().LastName, InitialData.Users.Last().PassPortNumber));
            }
        }
    }
}
