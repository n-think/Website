using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Image = Website.Core.Models.Domain.Image;

namespace Website.Services.Infrastructure.Validators
{
    public class ImageValidator : IShopValidator<Image>
    {
        public async Task<OperationResult> ValidateAsync(IShopManager manager, Image img)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (img?.BinData == null) throw new ArgumentNullException(nameof(img));

            var errors = new List<OperationError>();
            await Task.Run(() => ValidateImage(manager.Options.Image, img, manager, errors));

            if (errors.Count > 0)
            {
                return OperationResult.Failure(errors.ToArray());
            }

            return OperationResult.Success();
        }

        private void ValidateImage(ImageOptions options, Image img, IShopManager manager,
            ICollection<OperationError> errors)
        {
            if (img.ProductId <= 0)
            {
                errors.Add(manager.ErrorDescriber.InvalidModel());
            }
            
            if (!IsValidImage(img.BinData.FullData))
            {
                errors.Add(manager.ErrorDescriber.InvalidImageFormat());
            }
        }

        public bool IsValidImage(byte[] bytes)
        {
            try
            {
                using (var ms = new MemoryStream(bytes))
                {
                    var img = System.Drawing.Image.FromStream(ms);
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