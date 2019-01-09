using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Castle.Core.Internal;
using Microsoft.Extensions.Options;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;

namespace Website.Services.Infrastructure
{
    public class ShopImageTransformer : IShopImageTransformer<ImageBinData>
    {
        public ShopManagerOptions Options { get; private set; }

        public ShopImageTransformer(IOptions<ShopManagerOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value ?? throw new ArgumentNullException(nameof(Options));
        }

        public void ProcessImage(ImageBinData imageData)
        {
            if (imageData == null) throw new ArgumentNullException(nameof(imageData));
            if (!imageData.FullData.IsNullOrEmpty())
            {
                var maxHeight = Options.Image.MaxHeight;
                var maxWidth = Options.Image.MaxWidth;
                imageData.FullData = ScaleImage(imageData.FullData, maxWidth, maxHeight, Options.Image.SaveFormat);
            }

            if (!imageData.ThumbData.IsNullOrEmpty())
            {
                var maxThumbWidth = Options.Image.MaxThumbWidth;
                var maxThumbHeight = Options.Image.MaxThumbHeight;
                imageData.FullData = ScaleImage(imageData.FullData, maxThumbWidth, maxThumbHeight,
                    Options.Image.SaveFormat);
            }
        }

        private static Bitmap ScaleBitmap(Bitmap image, int maxWidth, int maxHeight)
        {
            // check if image already fits
            if (image.Width < maxWidth && image.Height < maxHeight)
                return image;

            var ratioX = (double) maxWidth / image.Width;
            var ratioY = (double) maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int) (image.Width * ratio);
            var newHeight = (int) (image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        private byte[] ScaleImage(byte[] image, int maxWidth, int maxHeight, ImageFormat format)
        {
            Bitmap bmp;
            using (var ms = new MemoryStream(image))
            {
                bmp = new Bitmap(ms);
            }
            using (bmp)
            {
                bmp = ScaleBitmap(bmp, maxWidth, maxHeight); //TODO fix warning
                using (var stream = new MemoryStream())
                {
                    var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == format.Guid);
                    var encParams = new EncoderParameters()
                    {
                        Param = new[]
                        {
                            new EncoderParameter(Encoder.Quality, Options.Image.EncoderQuality)
                        }
                    };
                    bmp.Save(stream, encoder, encParams);
                    return stream.ToArray();
                }
            }
        }
    }
}