using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Website.Core.Interfaces.Services;
using Website.Services.Services;
using Xunit;

namespace Tests.Services.Initializer
{
    public class AdminAndRoleInitializer
    {
        [Fact]
        public async Task InitializeAdminAccount()
        {
            //arrange
            var server = TestFactory.GetServer();
            var userManager = server.GetRequiredService<IUserManager>();
            var initializer = server.GetRequiredService<IDatabaseInitializer>();

            //act
            await initializer.InitializeAdminAccountRolesAsync();
            var user = await userManager.FindByNameAsync("admin");

            Assert.True(user != null);
        }
        
        [Fact]
        public async Task InitializeRoles()
        {
            //arrange
            var server = TestFactory.GetServer();
            var roleManager = server.GetRequiredService<RoleManager>();
            var initializer = server.GetRequiredService<IDatabaseInitializer>();
            var expectedRoles = new List<string>() {"admin", "user", "admin_generated"}.OrderBy(x => x);

            //act
            await initializer.InitializeAdminAccountRolesAsync();
            var roles = (await roleManager.Roles.ToArrayAsync()).Select(x => x.Name).OrderBy(x => x);
            
            Assert.Equal(expectedRoles, roles);
        }
    }
}