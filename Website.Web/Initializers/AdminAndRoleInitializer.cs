using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Website.Service.DTO;
using Website.Service.Services;

namespace Website.Web.Initializers
{
    public class AdminAndRoleInitializer
    {
        public static async Task InitializeAsync(UserManager userManager,
            RoleManager roleManager)
        {
            string adminEmail = "admin@example.com";
            string password = "1qaz@WSX";

            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new RoleDTO("admin"));
            }

            if (await roleManager.FindByNameAsync("manager") == null)
            {
                await roleManager.CreateAsync(new RoleDTO("manager"));
            }

            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new RoleDTO("user"));
            }

            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                var admin = new UserDTO() { Id = Guid.NewGuid().ToString(), Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}
