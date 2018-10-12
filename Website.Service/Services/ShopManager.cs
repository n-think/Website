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
    public sealed class ShopManager : IDisposable, IStoreManager
    {
        public ShopManager(IShopStore<ProductDTO, Product> store, ILogger<ShopManager> logger, /*DbContext dbContext,*/ IHttpContextAccessor context, IHostingEnvironment environment, StoreErrorDescriber errorDescriber = null)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ErrorDescriber = errorDescriber ?? new StoreErrorDescriber();
            //_dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
            _cancel = context?.HttpContext?.RequestAborted ?? CancellationToken.None;
            HostingEnvironment = environment;
        }

        //private readonly DbContext _dbContext; // DI scoped context
        private IShopStore<ProductDTO, Product> Store;
        private StoreErrorDescriber ErrorDescriber;
        private ILogger<ShopManager> Logger;
        private IHostingEnvironment HostingEnvironment;
        private readonly CancellationToken _cancel;

        private CancellationToken CancellationToken
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

            Store.AutoSaveChanges = false;

            await Store.CreateProductAsync(product, CancellationToken);

            if (!product.CategoryName.IsNullOrEmpty())
            {
                //TODO add prod to cat
                await Store.CreateCategoryAsync(product.CategoryName, CancellationToken);
            }

            if (!product.Images.IsNullOrEmpty())
            {
                //TODO add images
            }

            Store.AutoSaveChanges = true;

            await Store.SaveChanges(CancellationToken);
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

            IQueryable<Product> prodQuery = Store.ProductsQueryable;

            // filter roles
            Store.FilterProducstTypeQuery(types, prodQuery);

            // searching
            Store.SearchProductsQuery(searchString, prodQuery);

            // ordering
            Store.OrderProductsQuery(sortPropName, prodQuery);

            // paginating
            int skip = (currPage - 1) * countPerPage;
            int take = countPerPage;
            int totalProductsN = await Store.CountThenSkipTakeQuery(skip, take, prodQuery, CancellationToken);

            IEnumerable<ProductDTO> productsDto = await Store.ExecuteProductsQuery(prodQuery, CancellationToken);

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
            this.Store?.Dispose();
            _disposed = true;

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}

