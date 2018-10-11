﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Website.Service.DTO;
using Website.Service.Interfaces;
using Website.Service.Services;

namespace Website.Web.Initializers
{
    public class AdminAndRoleInitializer
    {

        public static async Task InitializeAsync(IUserManager userManager,
            RoleManager roleManager, IConfiguration configuration)
        {
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new RoleDTO("admin"));
            }

            if (await roleManager.FindByNameAsync("admin_generated") == null)
            {
                await roleManager.CreateAsync(new RoleDTO("admin_generated"));
            }

            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new RoleDTO("user"));
            }

            string login = "admin";
            string email = "admin@admin.admin"; //email doesnt matter and must adhere to usermanager email format x@x.x
            string password = configuration.GetSection("AdminAccount").GetValue<string>("Password");

            var adm = await userManager.FindByNameAsync(login);
            if (adm != null)
            {
                //check if password matches config
                if (!await userManager.CheckPasswordAsync(adm, password))
                {
                    await userManager.DeleteAsync(adm);
                    await CreateAdmin(userManager, login, email, password);
                }
                //check role
                if (!await userManager.IsInRoleAsync(adm, "admin_generated"))
                {
                    await userManager.AddToRoleAsync(adm, "admin_generated");
                }
            }
            else
            {
                await CreateAdmin(userManager, login, email, password);
            }
        }

        private static async Task CreateAdmin(IUserManager userManager, string login, string email, string password)
        {
            var adminUser = new UserDTO() { UserName = login, Email = email };
            IdentityResult result = await userManager.CreateAsync(adminUser, password);
            if (result.Succeeded)
            {
                var claims = new[]
                {
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
                var updatedAdm = await userManager.FindByNameAsync("admin"); //get updated admin with updated conc stamp or we get concurrency error on next update
                await userManager.AddToRoleAsync(updatedAdm, "admin_generated");
            }
        }
    }
}
