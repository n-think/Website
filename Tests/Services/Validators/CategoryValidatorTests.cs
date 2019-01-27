using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;
using Xunit;

namespace Tests.Services.Validators
{
    public class CategoryValidatorTests
    {
        [Fact]
        public async Task CategoryCannotHaveEmptyName()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<Category>>();

            var catWithInvalidName1 = new Category() {Name = ""};
            var catWithInvalidName2 = new Category() {Name = null};

            //act
            var result1 = await validator.ValidateAsync(shopManager, catWithInvalidName1);
            var result2 = await validator.ValidateAsync(shopManager, catWithInvalidName2);

            Assert.True(!result1.Succeeded &&
                        result1.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.EmptyCategoryName)));
            Assert.True(!result2.Succeeded &&
                        result1.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.EmptyCategoryName)));
        }

        [Fact]
        public async Task CategoryCannotSelfReferenceParent()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<Category>>();

            var catWithSelfRefParent = new Category() {Name = "test", ParentId = 1, Id = 1};

            //act
            var result = await validator.ValidateAsync(shopManager, catWithSelfRefParent);

            Assert.True(!result.Succeeded &&
                        result.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.InvalidModel)));
        }
        
        [Fact]
        public async Task CategoryAlreadyExists()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<Category>>();
            var shopRepo = server.GetShopRepository();

            var category1 = new Category() {Name = "test"};
            var category2 = new Category() {Name = "test"};

            //act
            await shopRepo.CreateCategoryAsync(category1, CancellationToken.None);
            var result = await validator.ValidateAsync(shopManager, category2);

            Assert.True(!result.Succeeded &&
                        result.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.DuplicateCategoryName)));
        }
        
        [Fact]
        public async Task PassValidCategory()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<Category>>();

            var category = new Category() {Name = "test"};

            //act
            var result = await validator.ValidateAsync(shopManager, category);

            Assert.True(result.Succeeded);
        }
    }
}