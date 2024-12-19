using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Core;
using BuildingBlocks.EFCore;
using Identity.Data.Seed;
using Identity.Identity.Constants;
using Identity.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Integration.Test;

public class IdentityTestDataSeeder(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IEventDispatcher eventDispatcher
)
    : ITestDataSeeder
{
    public async Task SeedAllAsync()
    {
        await SeedRoles();
        await SeedUsers();
    }

    private async Task SeedRoles()
    {
        if (await roleManager.RoleExistsAsync(Constants.Role.Admin) == false)
        {
            await roleManager.CreateAsync(new Role { Name = Constants.Role.Admin });
        }

        if (await roleManager.RoleExistsAsync(Constants.Role.User) == false)
        {
            await roleManager.CreateAsync(new Role { Name = Constants.Role.User });
        }
    }

    private async Task SeedUsers()
    {
        if (await userManager.FindByNameAsync("samh") == null)
        {
            var result = await userManager.CreateAsync(InitialData.Users.First(), "Admin@123456");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(InitialData.Users.First(), Constants.Role.Admin);

                await eventDispatcher.SendAsync(new UserCreated(InitialData.Users.First().Id, InitialData.Users.First().FirstName + " " + InitialData.Users.First().LastName, InitialData.Users.First().PassPortNumber));
            }
        }

        if (await userManager.FindByNameAsync("meysamh2") == null)
        {
            var result = await userManager.CreateAsync(InitialData.Users.Last(), "User@123456");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(InitialData.Users.Last(), Constants.Role.User);

                await eventDispatcher.SendAsync(new UserCreated(InitialData.Users.Last().Id, InitialData.Users.Last().FirstName + " " + InitialData.Users.Last().LastName, InitialData.Users.Last().PassPortNumber));
            }
        }
    }
}
