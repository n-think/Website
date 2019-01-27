using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Extensions.Options;
using Website.Core.Infrastructure;
using Website.Core.Models.Domain;
using Website.Services.Infrastructure;
using Xunit;

namespace Tests.Services
{
    public class ImageTransformerTests
    {
        public System.Drawing.Image ByteArrayToImage(byte[] bmpBytes)
        {
            System.Drawing.Image image = null;
            using (MemoryStream stream = new MemoryStream(bmpBytes))
            {
                image = System.Drawing.Image.FromStream(stream);
            }

            return image;
        }

        [Theory]
        [ClassData(typeof(ImageTransformerTestDataImages))]
        public void CorrectlyTransformsWithDifferentImages(Image image)
        {
            //arrange
            IOptions<ShopManagerOptions> optionsAccessor = new OptionAccessor1();
            var imageTransformer = new ShopImageTransformer(optionsAccessor);
            var imgOptions = optionsAccessor.Value.Image;

            //act
            imageTransformer.ProcessImage(image);
            var fullImg = ByteArrayToImage(image.BinData.FullData);
            var thumbImg = ByteArrayToImage(image.BinData.ThumbData);


            Assert.True(fullImg.Width <= imgOptions.MaxWidth &&
                        fullImg.Height <= imgOptions.MaxHeight &&
                        fullImg.RawFormat.Equals(imgOptions.SaveFormat));
            Assert.True(thumbImg.Width <= imgOptions.MaxThumbWidth &&
                        thumbImg.Height <= imgOptions.MaxThumbHeight &&
                        thumbImg.RawFormat.Equals(imgOptions.SaveFormat));
        }

        [Theory]
        [ClassData(typeof(ImageTransformerTestDataOptions))]
        public void CorrectlyTransformsWithDifferentOptions(IOptions<ShopManagerOptions> optionsAccessor)
        {
            //arrange
            var imageTransformer = new ShopImageTransformer(optionsAccessor);
            var imgOptions = optionsAccessor.Value.Image;
            var image = new Image()
            {
                BinData = new ImageBinData()
                {
                    FullData = TestFactory.GenerateValidImageBytes(100, 100, ImageFormat.Png),
                    ThumbData = TestFactory.GenerateValidImageBytes(100, 100, ImageFormat.Png),
                }
            };

            //act
            imageTransformer.ProcessImage(image);
            var fullImg = ByteArrayToImage(image.BinData.FullData);
            var thumbImg = ByteArrayToImage(image.BinData.ThumbData);


            Assert.True(fullImg.Width <= imgOptions.MaxWidth &&
                        fullImg.Height <= imgOptions.MaxHeight &&
                        fullImg.RawFormat.Equals(imgOptions.SaveFormat));
            Assert.True(thumbImg.Width <= imgOptions.MaxThumbWidth &&
                        thumbImg.Height <= imgOptions.MaxThumbHeight &&
                        thumbImg.RawFormat.Equals(imgOptions.SaveFormat));
        }

        [Fact]
        public void DoesntTransformImageThatSatisfiesOptions()
        {
            //arrange
            IOptions<ShopManagerOptions> optionsAccessor = new OptionAccessor200x200x100x100xJpg();
            var imageTransformer = new ShopImageTransformer(optionsAccessor);
            var imgOptions = optionsAccessor.Value.Image;

            var fullData = TestFactory.GenerateValidImageBytes(100, 100, ImageFormat.Jpeg);
            var thumbData = TestFactory.GenerateValidImageBytes(100, 100, ImageFormat.Jpeg);
            
            var image = new Image()
            {
                BinData = new ImageBinData()
                {
                    FullData = fullData,
                    ThumbData = thumbData,
                }
            };
            
            //act
            imageTransformer.ProcessImage(image);
            
            Assert.True(image.BinData.FullData == fullData && image.BinData.ThumbData== thumbData);
        }
    }

    public class ImageTransformerTestDataOptions : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {new OptionAccessor1()};
            yield return new object[] {new OptionAccessor2()};
            yield return new object[] {new OptionAccessor3()};
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ImageTransformerTestDataImages : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new Image()
                {
                    BinData = new ImageBinData()
                    {
                        FullData = TestFactory.GenerateValidImageBytes(100, 100, ImageFormat.Png),
                        ThumbData = TestFactory.GenerateValidImageBytes(100, 100, ImageFormat.Png),
                    }
                }
            };
            yield return new object[]
            {
                new Image()
                {
                    BinData = new ImageBinData()
                    {
                        FullData = TestFactory.GenerateValidImageBytes(300, 300, ImageFormat.Jpeg),
                        ThumbData = TestFactory.GenerateValidImageBytes(220, 220, ImageFormat.Jpeg),
                    }
                }
            };
            yield return new object[]
            {
                new Image()
                {
                    BinData = new ImageBinData()
                    {
                        FullData = TestFactory.GenerateValidImageBytes(1100, 1100, ImageFormat.Png),
                        ThumbData = TestFactory.GenerateValidImageBytes(200, 200, ImageFormat.Png),
                    }
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class OptionAccessor200x200x100x100xJpg : IOptions<ShopManagerOptions>
    {
        ShopManagerOptions IOptions<ShopManagerOptions>.Value => new ShopManagerOptions()
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
    }
    
    class OptionAccessor1 : IOptions<ShopManagerOptions>
    {
        ShopManagerOptions IOptions<ShopManagerOptions>.Value => new ShopManagerOptions()
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
    }

    class OptionAccessor2 : IOptions<ShopManagerOptions>
    {
        ShopManagerOptions IOptions<ShopManagerOptions>.Value => new ShopManagerOptions()
        {
            Image = new ImageOptions()
            {
                SaveFormat = ImageFormat.Jpeg,
                EncoderQuality = 80L,
                MaxWidth = 90,
                MaxHeight = 90,
                MaxThumbWidth = 150,
                MaxThumbHeight = 150,
            }
        };
    }

    class OptionAccessor3 : IOptions<ShopManagerOptions>
    {
        ShopManagerOptions IOptions<ShopManagerOptions>.Value => new ShopManagerOptions()
        {
            Image = new ImageOptions()
            {
                SaveFormat = ImageFormat.Png,
                EncoderQuality = 80L,
                MaxWidth = 110,
                MaxHeight = 110,
                MaxThumbWidth = 150,
                MaxThumbHeight = 150,
            }
        };
    }
}