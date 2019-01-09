using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;
using Image = Website.Core.Models.Domain.Image;

namespace Website.Services.Infrastructure
{
    public class ImageValidator : IShopValidator<Image>
    {
        public ImageValidator(OperationErrorDescriber errorDescriber)
        {
            ErrorDescriber = errorDescriber ?? new OperationErrorDescriber();
        }

        private OperationErrorDescriber ErrorDescriber { get; set; }

        public async Task<OperationResult> ValidateAsync(IShopManager manager, Image img)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (img?.BinData == null) throw new ArgumentNullException(nameof(img));

            var errors = new List<OperationError>();
            await Task.Run(() => ValidateProductImages(manager.Options.Image, img, errors));

            if (errors.Count > 0)
            {
                return OperationResult.Failure(errors.ToArray());
            }

            return OperationResult.Success();
        }

        private void ValidateProductImages(ImageOptions options, Image img, ICollection<OperationError> errors)
        {

            if (!IsValidImage(img.BinData.FullData, options.SaveFormat) ||
                !IsValidImage(img.BinData.FullData, options.SaveFormat))
            {
                errors.Add(ErrorDescriber.InvalidImageFormat());
                return;
            }
        }

        public bool IsValidImage(byte[] bytes, ImageFormat format)
        {
            try
            {
                using (var ms = new MemoryStream(bytes))
                {
                    var img = System.Drawing.Image.FromStream(ms);
                    if (!img.RawFormat.Equals(format))
                    {
                        return false;
                    }
                }
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

//        private ImageFormat GetImageFormat(System.Drawing.Image img)
//        {
//            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
//                return System.Drawing.Imaging.ImageFormat.Jpeg;
//            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
//                return System.Drawing.Imaging.ImageFormat.Bmp;
//            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
//                return System.Drawing.Imaging.ImageFormat.Png;
//            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Emf))
//                return System.Drawing.Imaging.ImageFormat.Emf;
//            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Exif))
//                return System.Drawing.Imaging.ImageFormat.Exif;
//            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
//                return System.Drawing.Imaging.ImageFormat.Gif;
//            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Icon))
//                return System.Drawing.Imaging.ImageFormat.Icon;
//            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.MemoryBmp))
//                return System.Drawing.Imaging.ImageFormat.MemoryBmp;
//            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff))
//                return System.Drawing.Imaging.ImageFormat.Tiff;
//            else
//                return System.Drawing.Imaging.ImageFormat.Wmf;
//        }
    }
}