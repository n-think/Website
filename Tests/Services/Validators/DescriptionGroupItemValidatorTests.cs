using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;
using Xunit;

namespace Tests.Services.Validators
{
    public class DescriptionGroupItemValidatorTests
    {
        [Fact]
        public async Task DescriptionGroupItemCannotHaveEmptyName()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<DescriptionGroupItem>>();

            var descGrpWithInvalidName1 = new DescriptionGroupItem() {Name = ""};
            var descGrpWithInvalidName2 = new DescriptionGroupItem() {Name = null};

            //act
            var result1 = await validator.ValidateAsync(shopManager, descGrpWithInvalidName1);
            var result2 = await validator.ValidateAsync(shopManager, descGrpWithInvalidName2);

            Assert.True(!result1.Succeeded &&
                        result1.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.EmptyDescriptionGroupItemName)));
            Assert.True(!result2.Succeeded &&
                        result1.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.EmptyDescriptionGroupItemName)));
        }
        
        [Fact]
        public async Task CannotCreateDescItemWithoutGroup()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<DescriptionGroupItem>>();
            var descItem = new DescriptionGroupItem() {Name = "testGroupItem", DescriptionGroupId = 322};

            //act
            var result = await validator.ValidateAsync(shopManager, descItem);

            Assert.True(!result.Succeeded &&
                        result.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.EntityNotFound)));
        }
        
        [Fact]
        public async Task DescriptionGroupItemAlreadyExists()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<DescriptionGroupItem>>();
            var shopRepo = server.GetShopRepository();
            
            var descGrp = new DescriptionGroup() {Name = "testGroup"};
            await shopRepo.CreateDescriptionGroupAsync(descGrp, CancellationToken.None);
            var descItem1 = new DescriptionGroupItem() {Name = "testGroupItem", DescriptionGroupId = descGrp.Id};
            var descItem2 = new DescriptionGroupItem() {Name = "testGroupItem", DescriptionGroupId = descGrp.Id};

            //act
            await shopRepo.CreateDescriptionGroupItemAsync(descItem1, CancellationToken.None);
            var result = await validator.ValidateAsync(shopManager, descItem2);

            Assert.True(!result.Succeeded &&
                        result.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.DuplicateDescriptionGroupItemName)));
        }
        
        [Fact]
        public async Task PassValidDescriptionItem()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<DescriptionGroupItem>>();
            var shopRepo = server.GetShopRepository();
            
            var product = new Product(){Name = "testProduct"};
            await shopRepo.CreateProductAsync(product, CancellationToken.None);
            
            var descGrp = new DescriptionGroup() {Name = "testGroup"};
            await shopRepo.CreateDescriptionGroupAsync(descGrp, CancellationToken.None);
            
            var descItem = new DescriptionGroupItem() {Name = "testGroupItem", DescriptionGroupId = descGrp.Id};
            
            //act
            var result = await validator.ValidateAsync(shopManager, descItem);

            Assert.True(result.Succeeded);
        }
    }
}