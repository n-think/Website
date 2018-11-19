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
        public ShopManager(IShopStore<ProductDto, ProductImageDto, CategoryDto, DescriptionGroupDto, OrderDto> store,
            ILogger<ShopManager> logger, IHttpContextAccessor context, OperationErrorDescriber errorDescriber = null)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _errorDescriber = errorDescriber ?? new OperationErrorDescriber();
            CancellationToken = context?.HttpContext?.RequestAborted ?? CancellationToken.None;
        }

        private readonly IShopStore<ProductDto, ProductImageDto, CategoryDto, DescriptionGroupDto, OrderDto> _store;
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

            var validateModelResult = ValidateProductModel(product);
            if (!validateModelResult.Succeeded)
            {
                return validateModelResult;
            }

            if (!product.Images.IsNullOrEmpty())
            {
                var validateImagesResult = ValidateAndProcessImages(product.Images);
                if (!validateImagesResult.Succeeded)
                    return validateImagesResult;
            }

            var prodResult = await _store.CreateProductAsync(product, CancellationToken);
            if (!prodResult.Succeeded)
            {
                return prodResult;
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> UpdateProductAsync(ProductDto product) //TODO refactor into smaller
        {
            ThrowIfDisposed();
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var validateModelResult = ValidateProductModel(product);
            if (!validateModelResult.Succeeded)
            {
                return validateModelResult;
            }

            if (!product.Images.IsNullOrEmpty())
            {
                var imageResult = ValidateAndProcessImages(product.Images);
                if (!imageResult.Succeeded)
                    return imageResult;
            }

            var prodResult = await _store.UpdateProductAsync(product, CancellationToken);
            if (!prodResult.Succeeded)
                return prodResult;

            return OperationResult.Success();
        }

        private OperationResult ValidateProductModel(ProductDto product)
        {
            if (!product.Id.HasValue || product.Name.IsNullOrEmpty())
            {
                return OperationResult.Failure(_errorDescriber.InvalidModel());
            }

            bool hasErrors = false;
            foreach (var category in product.Categories)
            {
                if (category.Id <= 0)
                {
                    hasErrors = true;
                    break;
                }
            }

            if (!hasErrors)
            {
                foreach (var group in product.Descriptions)
                {
                    if (!group.Id.HasValue || group.Id < 0)
                        hasErrors = true;
                    break;
                }

                foreach (var descItem in product.Descriptions.SelectMany(x => x.Items))
                {
                    if (!descItem.Id.HasValue)
                    {
                        hasErrors = true;
                        break;
                    }
                }
            }

            if (hasErrors)
            {
                return OperationResult.Failure(_errorDescriber.InvalidModel());
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteProductAsync(ProductDto product)
        {
            ThrowIfDisposed();
            if (product?.Id == null)
                throw new ArgumentException(nameof(product));

            if (product.Available)
            {
                return OperationResult.Failure(_errorDescriber.CannotDeleteActiveProduct());
            }

            var result = await _store.DeleteProductAsync(product, CancellationToken);
            if (!result.Succeeded)
            {
                return OperationResult.Failure(_errorDescriber.ErrorDeletingProduct());
            }

            return OperationResult.Success();
        }

        public async Task<ProductDto> GetProductByIdAsync(int id, bool loadImages, bool loadDescriptions,
            bool loadCategories)
        {
            ThrowIfDisposed();
            var product =
                await _store.FindProductByIdAsync(id, loadImages, loadDescriptions, loadCategories, CancellationToken);
            return product;
        }

        public async Task<ProductDto> GetProductByNameAsync(string name, bool loadImages, bool loadDescriptions,
            bool loadCategories)
        {
            ThrowIfDisposed();
            var product = await _store.FindProductByNameAsync(name, loadImages, loadDescriptions, loadCategories,
                CancellationToken);
            return product;
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
                        return OperationResult.Failure(_errorDescriber.InvalidImageFormat());
                    }

                    var bitmap = GetBitmapFromDataUrl(imageDto.DataUrl);
                    if (bitmap == null)
                        return OperationResult.Failure(_errorDescriber.InvalidImageFormat());
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
            var regex = new Regex(
                @"^data\:(?<type>image\/(jpg|jpeg|gif|png|tiff|bmp));base64,(?<data>[A-Z0-9\+\/\=]+)$",
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

        public async Task<SortPageResult<ProductDto>> GetSortFilterPageAsync(ItemTypeSelector types,
            string searchString, string sortPropName, int currPage, int countPerPage)
        {
            ThrowIfDisposed();
            // check inputs
            if (sortPropName == null) throw new ArgumentNullException(nameof(sortPropName));
            if (countPerPage < 0) throw new ArgumentOutOfRangeException(nameof(countPerPage));
            if (currPage < 0) throw new ArgumentOutOfRangeException(nameof(currPage));
            if (!Enum.IsDefined(typeof(ItemTypeSelector), types))
                throw new InvalidEnumArgumentException(nameof(ItemTypeSelector), (int) types, typeof(ItemTypeSelector));

            SortPageResult<ProductDto> result = await
                _store.SortFilterPageResultAsync(types, searchString, sortPropName, currPage, countPerPage,
                    CancellationToken);

            return result;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(bool getProductCount = false)
        {
            ThrowIfDisposed();
            return await _store.GetAllCategories(getProductCount, CancellationToken);
        }

        public async Task<IEnumerable<DescriptionGroupDto>> GetDescriptionGroupsAsync()
        {
            ThrowIfDisposed();
            return await _store.GetDescriptionGroupsAsync(CancellationToken);
        }

        public async Task<IEnumerable<DescriptionItemDto>> GetDescriptionItemsAsync(int id)
        {
            ThrowIfDisposed();
            if (id < 0)
                throw new ArgumentException(nameof(id));
            return await _store.GetDescriptionItemsAsync(id, CancellationToken);
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