using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Website.Data.EF.Models;
using Website.Service.DTO;
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
            var mockLogger = new Mock<ILogger<ClientService>>();
            //mockLogger.Setup(x => x.Log());
            var memoryContext = GetContext();
            var testUser = new ApplicationUser(){UserName = "test@email", NormalizedEmail = "TEST@EMAIL"};
            var profile = new ClientProfileDTO() {Email = "test@email", FirstName = "testName"};
            var fakeProfile = new ClientProfileDTO() { Email = "test@email1", FirstName = "testName" };
            var clientServ = new ClientService(memoryContext, mockLogger.Object);
            var set = memoryContext.Set<ApplicationUser>();



            set.Add(testUser);
            memoryContext.SaveChanges();
            var resultFalse = clientServ.CreateOrUpdateProfileAsync(fakeProfile).Result;
            var resultTrue = clientServ.CreateOrUpdateProfileAsync(profile).Result;

            var users = set.FirstOrDefault();


            Assert.False(resultFalse.Succedeed);
            Assert.True(resultTrue.Succedeed);

        }


        //sqlite in-memory context
        private DbContext GetContext() => SqlLiteMemoryContext();
        private DbContext SqlLiteMemoryContext()
        {
            var options = new DbContextOptionsBuilder<WebsiteContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            var context = new WebsiteContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();
            return context;
        }
    }
}
