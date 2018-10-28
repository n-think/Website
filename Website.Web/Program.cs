using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Website.Service.Interfaces;
using Website.Service.Services;
using Website.Web.Infrastructure.Initializers;

namespace Website.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            #region Инициализация ролями и аккаунтом администратора

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<IUserManager>();
                    var rolesManager = services.GetRequiredService<RoleManager>();
                    var configManager = services.GetRequiredService<IConfiguration>();
                    await AdminAndRoleInitializer.InitializeAsync(userManager, rolesManager, configManager);
                    //TODO УБРАТЬ
                    // seed test users
                    //await DbUserProfileSeed.InitializeAsync(userManager, rolesManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while creating admin account or roles.");
                }
            }

            #endregion 

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
#if DEBUG
                .UseApplicationInsights()
#endif
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>();

    }
}
