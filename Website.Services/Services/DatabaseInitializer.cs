using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;
using Website.Services.Infrastructure;

namespace Website.Services.Services
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private IUserManager UserManager { get; }
        private RoleManager RoleManager { get; }
        private IShopManager ShopManager { get; }
        private IConfiguration Configuration { get; }
        private DbContext DbContext { get; }
        private ILogger<DatabaseInitializer> Logger { get; }
        private IHostingEnvironment HostingEnvironment { get; }

        public DatabaseInitializer(IUserManager userManager, RoleManager roleManager, IShopManager shopManager,
            IConfiguration configuration, DbContext dbContext, ILogger<DatabaseInitializer> logger,
            IHostingEnvironment hostingEnvironment)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            ShopManager = shopManager;
            Configuration = configuration;
            DbContext = dbContext;
            Logger = logger;
            HostingEnvironment = hostingEnvironment;
        }

        public async Task<int> GenerateUsersAsync(int count)
        {
            var userGenerator = new RandomUserGenerator();

            int generatedCount = 0;
            int failedAttempts = 0;

            while (generatedCount < count && failedAttempts < 3)
            {
                var users = userGenerator.GetRandomUsers(count - generatedCount);
                foreach (var user in users)
                {
                    var userResult = await UserManager.CreateAsync(user, "123456");
                    await UserManager.AddToRoleAsync(user, "user");
                    if (userResult.Succeeded)
                    {
                        generatedCount++;
                    }
                    else
                    {
                        LogResultErrors(userResult, LogLevel.Warning, "Error adding generated user.");
                        failedAttempts++;
                    }
                }
            }

            return generatedCount;
        }

        public async Task<int> GenerateItemsAsync(int count)
        {
            int generatedCount = 0;
            int failedAttempts = 0;

            var productGenerator = new RandomProductGenerator(HostingEnvironment.ContentRootPath+"\\TestImages");
            
            while (generatedCount < count && failedAttempts < 3)
            {
                var productsEnumerable = productGenerator.GetRandomProducts(count - generatedCount);
                var productList = productsEnumerable.ToList();

                await CreateOrUpdateRelatedDataAsync(productList);

                foreach (var product in productList)
                {

                    var catIds = product.ProductToCategory.Select(x => x.Category.Id);
                    var result = await ShopManager.CreateProductAsync(product, product.Images, catIds, product.Descriptions);

                    if (!result.Succeeded)
                    {
                        failedAttempts++;
                    }
                    else
                    {
                        generatedCount++;
                    }
                }
            }

            return generatedCount;
        }

        private async Task CreateOrUpdateRelatedDataAsync(List<Product> productList)
        {
            //categories
            var categories = productList
                .SelectMany(x => x.ProductToCategory)
                .Select(x => x.Category)
                .Distinct();

            foreach (var category in categories)
            {
                var dbCat = await ShopManager.GetCategoryByNameAsync(category.Name);

                if (dbCat == null)
                {
                    var result = await ShopManager.CreateCategoryAsync(category);
                    if (!result.Succeeded)
                    {
                        LogResultErrors(result, LogLevel.Warning, "Error creating generated category.");
                    }
                }
                else
                {
                    category.Id = dbCat.Id;
                }
            }
            
            //description groups
            var descGroups = productList
                .SelectMany(x => x.Descriptions)
                .Select(x => x.DescriptionGroupItem)
                .Select(x => x.DescriptionGroup)
                .Distinct();
            
            foreach (var group in descGroups)
            {
                var dbGroup = await ShopManager.GetDescriptionGroupByNameAsync(group.Name);

                if (dbGroup == null)
                {
                    var result = await ShopManager.CreateDescriptionGroupAsync(group);
                    if (!result.Succeeded)
                    {
                        LogResultErrors(result, LogLevel.Warning, "Error creating generated description group.");
                    }
                }
                else
                {
                    group.Id = dbGroup.Id;
                }
            }
            
            //description group items
            var descGroupItems = productList
                .SelectMany(x => x.Descriptions)
                .Select(x => x.DescriptionGroupItem)
                .Distinct();
            
            foreach (var groupItem in descGroupItems)
            {
                var dbGroupItem = await ShopManager.GetDescriptionItemByNameAsync(groupItem.Name);

                if (dbGroupItem == null)
                {
                    groupItem.DescriptionGroupId = groupItem.DescriptionGroup.Id;
                    var result = await ShopManager.CreateDescriptionItemAsync(groupItem);
                    if (!result.Succeeded)
                    {
                        LogResultErrors(result, LogLevel.Warning, "Error creating generated description item.");
                    }
                }
                else
                {
                    groupItem.Id = dbGroupItem.Id;
                    groupItem.DescriptionGroupId = dbGroupItem.DescriptionGroupId;
                }
            }
            
            //descriptions
            var descriptions = productList
                .SelectMany(x => x.Descriptions);
            
            foreach (var description in descriptions)
            {
                description.DescriptionGroupItemId = description.DescriptionGroupItem.Id;
            }
        }

        public async Task<OperationResult> DropCreateDatabase()
        {
            await DbContext.Database.EnsureDeletedAsync();
            await DbContext.Database.MigrateAsync();
            OperationResult result = await InitializeAdminAccountRolesAsync();
            if (!result.Succeeded)
            {
                LogResultErrors(result, LogLevel.Warning,
                    "Error initializing database with generated admin account or with default roles.");
            }

            return result;
        }

        public async Task<OperationResult> InitializeAdminAccountRolesAsync()
        {
            if (await RoleManager.FindByNameAsync("admin") == null)
            {
                await RoleManager.CreateAsync(new Role("admin"));
            }

            if (await RoleManager.FindByNameAsync("admin_generated") == null)
            {
                await RoleManager.CreateAsync(new Role("admin_generated"));
            }

            if (await RoleManager.FindByNameAsync("user") == null)
            {
                await RoleManager.CreateAsync(new Role("user"));
            }

            IdentityResult result;
            string login = "admin";
            string email = "";
            string password = Configuration.GetSection("AdminAccount").GetValue<string>("Password");

            var origUserValidators = UserManager.UserValidators.ToList();
            var origPassValidators = UserManager.PasswordValidators.ToList();
            UserManager.PasswordValidators.Clear();
            UserManager.UserValidators.Clear();

            var adm = await UserManager.FindByNameAsync(login);
            if (adm != null)
            {
                //check if password matches config
                if (!await UserManager.CheckPasswordAsync(adm, password))
                {
                    result = await UserManager.DeleteAsync(adm);

                    if (!result.Succeeded)
                    {
                        LogResultErrors(result, LogLevel.Warning,
                            "Error deleting generated admin account with mismatched config password.");
                    }

                    result = await CreateAdmin(login, email, password);

                    if (!result.Succeeded)
                    {
                        LogResultErrors(result, LogLevel.Warning,
                            "Error creating generated admin account after deleting one with mismatched config password.");
                    }
                }

                //check role
                if (!await UserManager.IsInRoleAsync(adm, "admin_generated"))
                {
                    result = await UserManager.AddToRoleAsync(adm, "admin_generated");

                    if (!result.Succeeded)
                    {
                        LogResultErrors(result, LogLevel.Warning,
                            "Error adding generated admin account to 'admin_generated' role.");
                    }
                }
            }
            else
            {
                result = await CreateAdmin(login, email, password);

                if (!result.Succeeded)
                {
                    LogResultErrors(result, LogLevel.Warning,
                        "Error creating generated admin account.");
                }
            }

            //restore validators
            foreach (var origPassValidator in origPassValidators)
            {
                UserManager.PasswordValidators.Add(origPassValidator);
            }

            foreach (var origUserValidator in origUserValidators)
            {
                UserManager.UserValidators.Add(origUserValidator);
            }

            return OperationResult.Success();
        }

        private void LogResultErrors(IdentityResult result, LogLevel logLevel, string message)
        {
            var errors = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
            Logger.Log(LogLevel.Warning, $"{message} {errors}");
        }

        private void LogResultErrors(OperationResult result, LogLevel logLevel, string message)
        {
            var errors = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
            Logger.Log(LogLevel.Warning, $"{message} {errors}");
        }

        private async Task<IdentityResult> CreateAdmin(string login, string email, string password)
        {
            var adminUser = new User() {UserName = login, Email = email};
            IdentityResult result = await UserManager.CreateAsync(adminUser, password);
            await UserManager.SetLockoutEnabledAsync(adminUser, false);
            if (!result.Succeeded)
            {
                return result;
            }

            result = await UserManager.AddToRoleAsync(adminUser, "admin_generated");
            if (!result.Succeeded)
            {
                return result;
            }

            return IdentityResult.Success;
        }
    }
}