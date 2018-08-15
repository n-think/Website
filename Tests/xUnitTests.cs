using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Website.Data.EF.Models;
using Website.Service.DTO;
using Website.Service.IdentityStores;
using Website.Service.Interfaces;
using Website.Service.Mapper;
using Website.Service.Services;
using Xunit;

namespace xUnitTests
{
    public class ClientProfileTests
    {
        //TODO test all cases with messages
        [Fact]
        public void CreateOrUpdateTest()
        {
            var testUser = new User() { UserName = "test@email", NormalizedEmail = "TEST@EMAIL" };
            var profile = new UserProfileDTO() { Email = "test@email", FirstName = "testName" };
            var fakeProfile = new UserProfileDTO() { Email = "test@email1", FirstName = "testName" };
            var customUserManager = GetUserManager();
            var set = testContext.Set<User>();

            set.Add(testUser);
            testContext.SaveChanges();
            var resultFalse = customUserManager.CreateOrUpdateProfileAsync(fakeProfile).Result;
            var resultTrue = customUserManager.CreateOrUpdateProfileAsync(profile).Result;

            Assert.False(resultFalse.Succedeed);
            Assert.True(resultTrue.Succedeed);
        }

        private DbContext testContext;
        //sqlite in-memory context
        //private DbContext GetContext() => SqlLiteMemoryContext();
        private DbContext SqlLiteMemoryContext()
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
            testContext = SqlLiteMemoryContext();
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()));
            var userStore = new CustomUserStore(testContext, mapper);
            var manager = new UserManager(userStore, null, null, null, null, null, null, null, null, mapper);
            return manager;
        }
    }
}
