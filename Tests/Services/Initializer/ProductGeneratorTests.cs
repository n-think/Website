using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Website.Core.Interfaces.Services;
using Website.Services.Infrastructure;
using Xunit;

namespace Tests.Services.Initializer
{
    public class ProductGeneratorTests
    {
        private string imagesPath = Path.GetFullPath(@"../../../") + @"\TestImages";
            
        [Fact]
        public void GeneratesProducts()
        {
            //arrange
            var prodGen = new RandomProductGenerator(imagesPath);
            var genCount = 3;

            //act
            var prods = prodGen.GetRandomProducts(genCount);
            
            Assert.True(prods.Count() == genCount);
        }
        
        [Fact]
        public async Task GeneratesValidProduct()
        {
            //arrange
            var server = TestFactory.GetServer();
            var manager = server.GetRequiredService<IShopManager>();
            var initializer = server.GetRequiredService<IDatabaseInitializer>();
            var genCount = 3;

            //act
            await initializer.GenerateItemsAsync(genCount);
            var countInDb = (await manager.GetNewProducts(genCount)).Count();

            Assert.True(countInDb == genCount);
        }
    }
}