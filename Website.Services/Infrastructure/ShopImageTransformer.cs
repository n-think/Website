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
using Image = Website.Core.Models.Domain.Image;

namespace Website.Services.Infrastructure
{
    //does NOT crop
    public class ShopImageTransformer : IShopImageTransformer<Image>
    {
        public ShopManagerOptions Options { get; private set; }

        public ShopImageTransformer(IOptions<ShopManagerOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value ?? throw new ArgumentNullException(nameof(Options));
        }

        public void ProcessImage(Image image)
        {
            var imageData = image.BinData ?? throw new ArgumentException(nameof(image));

            if (imageData == null) throw new ArgumentNullException(nameof(imageData));
            if (!imageData.FullData.IsNullOrEmpty())
            {
                var maxHeight = Options.Image.MaxHeight;
                var maxWidth = Options.Image.MaxWidth;
                imageData.FullData = ConvertImage(imageData.FullData, maxWidth, maxHeight, Options.Image.SaveFormat);
            }

            if (!imageData.ThumbData.IsNullOrEmpty())
            {
                var maxThumbWidth = Options.Image.MaxThumbWidth;
                var maxThumbHeight = Options.Image.MaxThumbHeight;
                imageData.ThumbData = ConvertImage(imageData.ThumbData, maxThumbWidth, maxThumbHeight,
                    Options.Image.SaveFormat);
            }

            image.Mime = "image/" + Options.Image.SaveFormat.ToString().ToLower();
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

        private byte[] ConvertImage(byte[] image, int maxWidth, int maxHeight, ImageFormat format)
        {
            using (var ms = new MemoryStream(image))
            {
                using (var bmp = new Bitmap(ms))
                {
                    var modified = bmp;
                    if (bmp.Height > maxHeight || bmp.Width > maxWidth)
                    {
                        modified = ScaleBitmap(bmp, maxWidth, maxHeight);
                    }

                    if (modified.RawFormat.Equals(format))
                        return image;
                    
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
                        modified.Save(stream, encoder, encParams);
                        return stream.ToArray();
                    }
                }
            }
        }
    }
}