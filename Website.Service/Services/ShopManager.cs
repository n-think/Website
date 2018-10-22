using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

namespace Website.Service.Services
{
    public class ShopManager : IDisposable, IShopManager
    {
        public ShopManager(IShopStore<ProductDto, ProductImageDto, OrderDto> store, ILogger<ShopManager> logger, IHttpContextAccessor context,  OperationErrorDescriber errorDescriber = null)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _errorDescriber = errorDescriber ?? new OperationErrorDescriber();
            CancellationToken = context?.HttpContext?.RequestAborted ?? CancellationToken.None;
        }

        private readonly IShopStore<ProductDto, ProductImageDto, OrderDto> _store;
        private readonly OperationErrorDescriber _errorDescriber;
        private readonly ILogger<ShopManager> _logger;
        

        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// Creates product with images and category in db.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<OperationResult> CreateItemAsync(ProductDto product)
        {
            ThrowIfDisposed();
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            _store.AutoSaveChanges = false;

            await _store.CreateProductAsync(product, CancellationToken);

            if (!product.Categories.IsNullOrEmpty())
            {
                //TODO add prod to cat
                //await _store.CreateCategoryAsync(product.CategoryName, CancellationToken);
            }

            if (product != null)
            {
                //TODO add images
            }

            _store.AutoSaveChanges = true;

            await _store.SaveChanges(CancellationToken);
            return OperationResult.Success();
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

        public async Task<ProductDto> GetProductById(int id)
        {
            ThrowIfDisposed();
            return await _store.FindProductByIdAsync(id, CancellationToken);
        }

        public async Task<List<DescriptionGroupDto>> GetProductDescriptions(int productId)
        {
            ThrowIfDisposed();

            return await _store.GetProductDescriptions(productId, CancellationToken); ;
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

