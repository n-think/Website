using System.Linq;
using System.Threading.Tasks;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;
using Xunit;

namespace Tests.Services.Validators
{
    public class ProductValidatorTests
    {
        [Fact]
        public async Task ProductNameCannotBeEmpty()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<Product>>();

            var prod1 = new Product() {Name = ""};
            var prod2 = new Product() {Name = null};

            //act
            var result1 = await validator.ValidateAsync(shopManager, prod1);
            var result2 = await validator.ValidateAsync(shopManager, prod2);

            Assert.True(!result1.Succeeded &&
                        result1.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.EmptyProductName)));
            Assert.True(!result2.Succeeded &&
                        result1.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.EmptyProductName)));
        }
        
        [Fact]
        public async Task ProductCodeCannotBeDefaultValue()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<Product>>();

            var prod1 = new Product() {Code = default(int)};

            //act
            var result = await validator.ValidateAsync(shopManager, prod1);

            Assert.True(!result.Succeeded &&
                        result.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.EmptyProductCode)));
        }
        
        
        [Fact]
        public async Task ProductCodeAlreadyExists()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<Product>>();

            var prod1 = new Product() {Name = "testProduct", Code = 1};
            var prod2 = new Product() {Name = "testProduct", Code = 1};

            //act
            await shopManager.CreateProductAsync(prod1, null, null, null);
            var result = await validator.ValidateAsync(shopManager, prod2);

            Assert.True(!result.Succeeded &&
                        result.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.DuplicateProductCode)));
        }
        
        [Fact]
        public async Task ProductNameAlreadyExists()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<Product>>();

            var prod1 = new Product() {Name = "testProduct", Code = 1};
            var prod2 = new Product() {Name = "testProduct", Code = 1};

            //act
            await shopManager.CreateProductAsync(prod1, null, null, null);
            var result = await validator.ValidateAsync(shopManager, prod2);

            Assert.True(!result.Succeeded &&
                        result.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.DuplicateProductName)));
        }
        
        [Fact]
        public async Task PassValidProduct()
        {
            //arrange
            var server = TestFactory.GetServer();
            var shopManager = server.GetRequiredService<IShopManager>();
            var validator = server.GetRequiredService<IShopValidator<Product>>();

            var prod = new Product() {Name = "testProduct", Code = 1};

            //act
            var result = await validator.ValidateAsync(shopManager, prod);

            Assert.True(result.Succeeded);
        }
    }
}