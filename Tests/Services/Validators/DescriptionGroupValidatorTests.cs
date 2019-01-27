using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;
using Xunit;

namespace Tests.Services.Validators
{
    public class DescriptionGroupValidatorTests
    {
        [Fact]
        public async Task DescriptionGroupCannotHaveEmptyName()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<DescriptionGroup>>();

            var descGrpWithInvalidName1 = new DescriptionGroup() {Name = ""};
            var descGrpWithInvalidName2 = new DescriptionGroup() {Name = null};

            //act
            var result1 = await validator.ValidateAsync(shopManager, descGrpWithInvalidName1);
            var result2 = await validator.ValidateAsync(shopManager, descGrpWithInvalidName2);

            Assert.True(!result1.Succeeded &&
                        result1.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.EmptyDescriptionGroupName)));
            Assert.True(!result2.Succeeded &&
                        result1.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.EmptyDescriptionGroupName)));
        }
        
        [Fact]
        public async Task DescriptionGroupAlreadyExists()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<DescriptionGroup>>();
            var shopRepo = server.GetShopRepository();

            var descGrp1 = new DescriptionGroup() {Name = "testGroup"};
            var descGrp2 = new DescriptionGroup() {Name = "testGroup"};

            //act
            await shopRepo.CreateDescriptionGroupAsync(descGrp1, CancellationToken.None);
            var result = await validator.ValidateAsync(shopManager, descGrp2);

            Assert.True(!result.Succeeded &&
                        result.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.DuplicateDescriptionGroupName)));
        }
        
        [Fact]
        public async Task PassValidDescriptionGroup()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<DescriptionGroup>>();
            var shopRepo = server.GetShopRepository();

            var descGrp = new DescriptionGroup() {Name = "testGroup"};

            //act
            var result = await validator.ValidateAsync(shopManager, descGrp);

            Assert.True(result.Succeeded);
        }
    }
}