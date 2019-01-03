using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Website.Core.DTO;
using Website.Core.Models;
using Website.Core.Models.Domain;
using Website.Data.EF;
using Website.Services.Mapper;
using Website.Services.Services;
using Xunit;

namespace Tests
{
    public class ClientProfileTests
    {
        //TODO test all cases with messages
        [Fact]
        public void CreateOrUpdateProfileTest()
        {
            var testUser = new User() { UserName = "test@email", Email = "test@email", NormalizedUserName = "TEST@EMAIL" };
            var profile = new UserProfileDto() { Login = "test@email", FirstName = "testName" };
            var fakeProfile = new UserProfileDto() { Login = "test@email1", FirstName = "testName" };
            var customUserManager = GetUserManager();
            var set = _testContext.Set<User>();

            set.Add(testUser);
            _testContext.SaveChanges();
            var resultFalse = customUserManager.CreateOrUpdateProfileAsync(fakeProfile).Result;
            var resultTrue = customUserManager.CreateOrUpdateProfileAsync(profile).Result;

            Assert.False(resultFalse.Succeeded);
            Assert.True(resultTrue.Succeeded);
        }

        private DbContext _testContext;
        //sqlite in-memory context
        private DbContext GetSqlLiteMemoryContext()
        {
            var options = new DbContextOptionsBuilder<WebsiteDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            var context = new WebsiteDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();
            return context;
        }
        private UserManager GetUserManager()
        {
            _testContext = GetSqlLiteMemoryContext();
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<ServiceProfile>()));
            var userStore = new Mock<IUserStore<UserDto>>();
            var manager = new UserManager(_testContext, userStore.Object, null, null, null, null, null, null, null, null, mapper);
            return manager;
        }
    }
}
