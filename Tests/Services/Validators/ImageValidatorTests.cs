using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;
using Website.Services.Infrastructure.Validators;
using Xunit;
using Image = Website.Core.Models.Domain.Image;

namespace Tests.Services.Validators
{
    public class ImageValidatorTests
    {
        private IShopManager GetTestShopManager()
        {
            var mock = new Mock<IShopManager>();

            var opts = new ShopManagerOptions()
            {
                Image = new ImageOptions()
                {
                    SaveFormat = ImageFormat.Jpeg,
                    EncoderQuality = 80L,
                    MaxWidth = 1000,
                    MaxHeight = 1000,
                    MaxThumbWidth = 150,
                    MaxThumbHeight = 150,
                }
            };

            mock.Setup(x => x.Options).Returns(opts);
            mock.Setup(x => x.ErrorDescriber).Returns(new OperationErrorDescriber());
            return mock.Object;
        }

        [Fact]
        public async Task ImageMustHaveValidBinData()
        {
            //arrange
            var manager = GetTestShopManager();
            var validator = new ImageValidator();

            var image = new Image {ProductId = 1, BinData = new ImageBinData()};

            //act
            var result = await validator.ValidateAsync(manager, image);

            Assert.True(!result.Succeeded &&
                        result.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.InvalidImageFormat)));
        }

        [Fact]
        public async Task PassValidImage()
        {
            //arrange
            var manager = GetTestShopManager();
            var validator = new ImageValidator();

            var imageBytes = TestFactory.GenerateValidImageBytes(1001, 1001, ImageFormat.Png);
            var image = new Image()
            {
                ProductId = 322,
                BinData = new ImageBinData()
                {
                    FullData = imageBytes
                }
            };

            //act
            var result = await validator.ValidateAsync(manager, image);

            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task RejectInvalidImage()
        {
            //arrange
            var manager = GetTestShopManager();
            var validator = new ImageValidator();

            var image = new Image()
            {
                ProductId = 322,
                BinData = new ImageBinData()
                {
                    FullData = new byte[10]
                }
            };

            //act
            var result = await validator.ValidateAsync(manager, image);

            Assert.True(!result.Succeeded &&
                        result.Errors.Any(x => x.Code == nameof(OperationErrorDescriber.InvalidImageFormat)));
        }
    }
}