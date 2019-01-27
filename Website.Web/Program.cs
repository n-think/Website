using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Website.Core.Interfaces.Services;

namespace Website.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                #region dbMigrations

                var context = services.GetRequiredService<DbContext>();
                await context.Database.MigrateAsync();

                #endregion
                
                #region Инициализация ролями и аккаунтом администратора
                try
                {
                    var initializer = services.GetRequiredService<IDatabaseInitializer>();
                    await initializer.InitializeAdminAccountRolesAsync();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while creating admin account or roles.");
                }
                #endregion
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder<Startup>(args);
    }
}