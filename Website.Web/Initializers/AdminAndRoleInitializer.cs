using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            string login = "admin";
            string email = "admin@admin.admin";
            string password = "1qaz@WSX";

            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new RoleDTO("admin"));
            }

            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new RoleDTO("user"));
            }

            if (await userManager.FindByNameAsync(login) == null)
            {
                var adminUser = new UserDTO() { UserName = login, Email = email };
                IdentityResult result = await userManager.CreateAsync(adminUser, password);
                if (result.Succeeded)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Role, "admin"),
                        new Claim("ViewUsers", ""),
                        new Claim("EditUsers", ""),
                        new Claim("DeleteUsers", ""),
                        new Claim("ViewItems", ""),
                        new Claim("EditItems", ""),
                        new Claim("CreateItems", ""),
                        new Claim("DeleteItems", ""),
                        new Claim("EditUsers", ""),
                        new Claim("ViewOrders", ""),
                        new Claim("EditOrders", ""),
                        new Claim("DeleteOrders", "")
                    };
                    await userManager.AddClaimsAsync(adminUser, claims);
                }
            }
        }
    }
}
