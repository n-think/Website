using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;
using Xunit;

namespace Tests.Services.Validators
{
    public class DescriptionValidatorTests
    {
        [Fact]
        public async Task DescriptionCannotBeEmpty()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<Description>>();

            var descWithInvalidName1 = new Description() {Value = ""};
            var descWithInvalidName2 = new Description() {Value = null};

            //act
            var result1 = await validator.ValidateAsync(shopManager, descWithInvalidName1);
            var result2 = await validator.ValidateAsync(shopManager, descWithInvalidName2);

            Assert.True(!result1.Succeeded &&
                        result1.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.EmptyDescriptionValue)));
            Assert.True(!result2.Succeeded &&
                        result1.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.EmptyDescriptionValue)));
        }

        [Fact]
        public async Task DescriptionWithoutReferenceIds()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<Description>>();
            var desc = new Description() {Value = "testDescription"};

            //act
            var result = await validator.ValidateAsync(shopManager, desc);

            Assert.True(!result.Succeeded &&
                        result.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.InvalidModel)));
        }

        [Fact]
        public async Task DescriptionWithoutExistingReferences()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<Description>>();
            var shopRepo = server.GetShopRepository();
            
            var product = new Product(){Name = "testProduct"};
            await shopRepo.CreateProductAsync(product, CancellationToken.None);
            
            var descGrp = new DescriptionGroup() {Name = "testGroup"};
            await shopRepo.CreateDescriptionGroupAsync(descGrp, CancellationToken.None);
            
            var descItem = new DescriptionGroupItem() {Name = "testGroupItem", DescriptionGroupId = descGrp.Id};
            await shopRepo.CreateDescriptionGroupItemAsync(descItem, CancellationToken.None);

            var desc1 = new Description() {Value = "testDescription", ProductId = product.Id, DescriptionGroupItemId = 322};
            var desc2 = new Description() {Value = "testDescription", ProductId = 322, DescriptionGroupItemId = descItem.Id};

            //act
            var result1 = await validator.ValidateAsync(shopManager, desc1);
            var result2 = await validator.ValidateAsync(shopManager, desc2);

            Assert.True(!result1.Succeeded &&
                        result1.Errors.Where(x => x.Code == nameof(OperationErrorDescriber.EntityNotFound)).Take(2).Count() == 1);
            Assert.True(!result1.Succeeded &&
                        result1.Errors.Where(x => x.Code == nameof(OperationErrorDescriber.EntityNotFound)).Take(2).Count() == 1);
        }
        
        [Fact]
        public async Task PassValidDescription()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<Description>>();
            var shopRepo = server.GetShopRepository();
            
            var product = new Product(){Name = "testProduct"};
            await shopRepo.CreateProductAsync(product, CancellationToken.None);
            
            var descGrp = new DescriptionGroup() {Name = "testGroup"};
            await shopRepo.CreateDescriptionGroupAsync(descGrp, CancellationToken.None);
            
            var descItem = new DescriptionGroupItem() {Name = "testGroupItem", DescriptionGroupId = descGrp.Id};
            await shopRepo.CreateDescriptionGroupItemAsync(descItem, CancellationToken.None);

            var desc = new Description() {Value = "testDescription", ProductId = product.Id, DescriptionGroupItemId = descItem.Id};

            //act
            var result = await validator.ValidateAsync(shopManager, desc);

            Assert.True(result.Succeeded);
        }
    }
}