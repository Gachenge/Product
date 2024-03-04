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
                var userEmail = "admin@abno.com";
                var poweruser = new User
                {
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true,
                    Role = UserRole.Admin
                };
                string password = "123456";
                var user = await userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    var createPowerUser = await userManager.CreateAsync(poweruser, password);
                    //if (createPowerUser.Succeeded)
                    //{
                    //    await userManager.AddToRoleAsync(poweruser, "Admin");
                    //}
                }
            }
        }
    }
}
