using System.Linq;
using System.Threading.Tasks;
using Website.Core.Interfaces.Services;
using Website.Services.Infrastructure;
using Xunit;

namespace Tests.Services.Initializer
{
    public class UserGeneratorTests
    {
        [Fact]
        public void GeneratesUsers()
        {
            //arrange
            var userGen = new RandomUserGenerator();
            var genCount = 7;

            //act
            var users = userGen.GetRandomUsers(genCount);

            Assert.True(users.Count() == genCount);
        }

        [Fact]
        public async Task GeneratesValidUser()
        {
            //arrange
            var userGen = new RandomUserGenerator();
            var user = userGen.GetRandomUsers(1).Single();
            var server = TestFactory.GetServer();
            var initializer = server.GetRequiredService<IDatabaseInitializer>();
            var userManager = server.GetRequiredService<IUserManager>();
            var genCount = 7;

            //act
            await initializer.InitializeAdminAccountRolesAsync();
            await initializer.GenerateUsersAsync(genCount);
            var userCountInDb = userManager.Users.Count(x => x.UserRoles.Any(y => y.Role.Name == "user"));

            Assert.True(userCountInDb == genCount);
        }
    }
}