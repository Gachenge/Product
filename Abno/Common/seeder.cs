using Abno.Data;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Abno.Models;

namespace Abno.Common
{
    public class Seeder
    {
        public async Task IdentitySeed(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roleNames = { "Admin", "User" };
                IdentityResult roleResult;

                foreach (var roleName in roleNames)
                {
                    var roleExist = await RoleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
                var userEmail = "admin@abno.com";
                var poweruser = new User
                {
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true,
                };
                string password = "123456";
                var user = await userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    var createPowerUser = await userManager.CreateAsync(poweruser, password);
                    if (createPowerUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(poweruser, "Admin");
                    }
                }
            }
        }
    }
}
