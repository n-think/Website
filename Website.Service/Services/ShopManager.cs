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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Website.Data.EF.Models;
using Website.Service.DTO;
using Website.Service.Enums;
using Website.Service.Infrastructure;
using Website.Service.Interfaces;
using Website.Service.Stores;

namespace Website.Service.Services
{
    public class ShopManager : IDisposable, IStoreManager
    {
        public ShopManager(IShopStore<ProductDTO, Product> store, ILogger<ShopManager> logger, IHttpContextAccessor context, IHostingEnvironment environment, StoreErrorDescriber errorDescriber = null)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _errorDescriber = errorDescriber ?? new StoreErrorDescriber();
            _cancel = context?.HttpContext?.RequestAborted ?? CancellationToken.None;
            _hostingEnvironment = environment;
        }
        
        private readonly IShopStore<ProductDTO, Product> _store;
        private readonly StoreErrorDescriber _errorDescriber;
        private readonly ILogger<ShopManager> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly CancellationToken _cancel;

        public CancellationToken CancellationToken
        {
            get { return this._cancel; }
        }

        /// <summary>
        /// Creates product with images and category in db.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OperationResult> CreateItemAsync(ProductDTO product)
        {
            ThrowIfDisposed();
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            _store.AutoSaveChanges = false;

            await _store.CreateProductAsync(product, CancellationToken);

            if (!product.CategoryName.IsNullOrEmpty())
            {
                //TODO add prod to cat
                await _store.CreateCategoryAsync(product.CategoryName, CancellationToken);
            }

            if (!product.Images.IsNullOrEmpty())
            {
                //TODO add images
            }

            _store.AutoSaveChanges = true;

            await _store.SaveChanges(CancellationToken);
            return OperationResult.Success();
        }

        public async Task<SortPageResult<ProductDTO>> GetSortFilterPageAsync(ItemTypeSelector types, string searchString, string sortPropName, int currPage, int countPerPage)
        {
            ThrowIfDisposed();
            // check inputs
            if (sortPropName == null) throw new ArgumentNullException(nameof(sortPropName));
            if (countPerPage < 0) throw new ArgumentOutOfRangeException(nameof(countPerPage));
            if (currPage < 0) throw new ArgumentOutOfRangeException(nameof(currPage));
            if (!Enum.IsDefined(typeof(ItemTypeSelector), types))
                throw new InvalidEnumArgumentException(nameof(ItemTypeSelector), (int)types, typeof(ItemTypeSelector));

            IQueryable<Product> prodQuery = _store.ProductsQueryable;

            // filter roles
            _store.FilterProducstTypeQuery(types, ref prodQuery);

            // searching
            _store.SearchProductsQuery(searchString, ref prodQuery);

            // ordering
            _store.OrderProductsQuery(sortPropName, ref prodQuery);

            // paginating
            int skip = (currPage - 1) * countPerPage;
            int take = countPerPage;
            int totalProductsN = await _store.CountQueryAsync(prodQuery, CancellationToken);
            _store.SkipTakeQuery(skip, take, ref prodQuery);

            IEnumerable<ProductDTO> productsDto = await _store.ExecuteProductsQuery(prodQuery, CancellationToken);
            return new SortPageResult<ProductDTO> { FilteredData = productsDto, TotalN = totalProductsN };
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

