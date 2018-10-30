using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Website.Service.DTO;
using Website.Service.Enums;
using Website.Service.Infrastructure;
using Website.Service.Interfaces;
using Website.Service.Stores;

namespace Website.Service.Services
{
    public class ShopManager : IDisposable, IShopManager
    {
        public ShopManager(IShopStore<ProductDto, ProductImageDto, CategoryDto, OrderDto> store, ILogger<ShopManager> logger, IHttpContextAccessor context, OperationErrorDescriber errorDescriber = null)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _errorDescriber = errorDescriber ?? new OperationErrorDescriber();
            CancellationToken = context?.HttpContext?.RequestAborted ?? CancellationToken.None;
        }

        private readonly IShopStore<ProductDto, ProductImageDto, CategoryDto, OrderDto> _store;
        private readonly OperationErrorDescriber _errorDescriber;
        private readonly ILogger<ShopManager> _logger;


        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// Creates product with images and category in db.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<OperationResult> CreateProductAsync(ProductDto product)
        {
            ThrowIfDisposed();
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var prodResult = await _store.CreateProductAsync(product, CancellationToken);

            if (!product.Categories.IsNullOrEmpty())
            {
                //TODO add prod to cat
                //await _store.AddProductToCategoryAsync(product, category, CancellationToken);
            }

            var imageResult = OperationResult.Success();
            if (!product.Images.IsNullOrEmpty())
            {
                //TODO 
                //imageResult = await _store.SaveImagesAsync(product, CancellationToken)
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteProductAsync(ProductDto product)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductDto> GetProductById(int id, bool loadDescriptions)
        {
            ThrowIfDisposed();
            var product = await _store.FindProductByIdAsync(id, CancellationToken);
            if (loadDescriptions)
            {
                await _store.LoadProductDescriptions(product, CancellationToken);
            }
            return product;
        }

        public async Task<OperationResult> UpdateProductAsync(ProductDto product) //TODO refactor into smaller
        {
            ThrowIfDisposed();
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var prodResult = await _store.UpdateProductAsync(product, CancellationToken);
            if (!prodResult.Succeeded)
                return prodResult;

            var catResult = OperationResult.Success();
            if (!product.Categories.IsNullOrEmpty())
            {
                //TODO add prod to cat
                //await _store.AddProductToCategoryAsync(product, category, CancellationToken);
            }

            if (!product.Images.IsNullOrEmpty())
            {
                var imageResult = ValidateAndProcessImages(product.Images);
                if (!imageResult.Succeeded)
                    return imageResult;

                imageResult = await _store.SaveImagesAsync(product, CancellationToken);
                if (!imageResult.Succeeded)
                    return imageResult;
            }

            return OperationResult.Success();
        }

        private OperationResult ValidateAndProcessImages(List<ProductImageDto> productImages)
        {
            int count = 0;
            foreach (var imageDto in productImages)
            {
                if (imageDto.DtoState == DtoState.Added)
                {
                    if (ASCIIEncoding.ASCII.GetByteCount(imageDto.DataUrl) > 5242880)
                    {
                        productImages.RemoveAt(count);
                        return OperationResult.Failure(_errorDescriber.IncorrectImageFormat());
                    }

                    var bitmap = GetBitmapFromDataUrl(imageDto.DataUrl);
                    if (bitmap == null)
                        return OperationResult.Failure(_errorDescriber.IncorrectImageFormat());
                    if (Math.Max(bitmap.Height, bitmap.Width) > 1000)
                        bitmap = StoreHelpers.ScaleImage(bitmap, 1000, 1000);
                    imageDto.Bitmap = bitmap;
                }
                count++;
            }
            return OperationResult.Success();
        }

        private Bitmap GetBitmapFromDataUrl(string dataUrl)
        {
            var regex = new Regex(@"^data\:(?<type>image\/(jpg|jpeg|gif|png|tiff|bmp));base64,(?<data>[A-Z0-9\+\/\=]+)$",
                RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
            var match = regex.Match(dataUrl);
            if (!match.Success)
                return null;

            string mimeType = match.Groups["type"].Value;
            string base64Data = match.Groups["data"].Value;
            byte[] rawData;
            try
            {
                rawData = Convert.FromBase64String(base64Data);
            }
            catch (FormatException e)
            {
                _logger.LogInformation(e, "Error converting image from DataUrl.");
                return null;
            }
            var memoryStream = new MemoryStream(rawData);
            Image image;
            try
            {
                image = Image.FromStream(memoryStream, false, true);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "Error converting image from DataUrl.");
                return null;
            }
            return new Bitmap(image);
        }

        public async Task<SortPageResult<ProductDto>> GetSortFilterPageAsync(ItemTypeSelector types, string searchString, string sortPropName, int currPage, int countPerPage)
        {
            ThrowIfDisposed();
            // check inputs
            if (sortPropName == null) throw new ArgumentNullException(nameof(sortPropName));
            if (countPerPage < 0) throw new ArgumentOutOfRangeException(nameof(countPerPage));
            if (currPage < 0) throw new ArgumentOutOfRangeException(nameof(currPage));
            if (!Enum.IsDefined(typeof(ItemTypeSelector), types))
                throw new InvalidEnumArgumentException(nameof(ItemTypeSelector), (int)types, typeof(ItemTypeSelector));

            SortPageResult<ProductDto> result = await
                _store.SortFilterPageResultAsync(types, searchString, sortPropName, currPage, countPerPage, CancellationToken);

            return result;
        }

        /// <summary>Throws if this class has been disposed.</summary>
        private void ThrowIfDisposed()
        {
            if (this._disposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        #region IDisposable

        private bool _disposed = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposing || this._disposed)
                return;

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.
            this._store?.Dispose();
            _disposed = true;

        }

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        #endregion

    }
}

