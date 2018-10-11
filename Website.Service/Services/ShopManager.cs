using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.Extensions.Logging;
using Website.Service.DTO;
using Website.Service.Infrastructure;
using Website.Service.Interfaces;
using Website.Service.Stores;

namespace Website.Service.Services
{
    public class ShopManager : IDisposable, IStoreManager
    {
        public ShopManager(IShopStore<ProductDTO> store, ILogger logger, StoreErrorDescriber errorDescriber = null)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ErrorDescriber = errorDescriber ?? new StoreErrorDescriber();
        }

        private IShopStore<ProductDTO> Store;
        private StoreErrorDescriber ErrorDescriber;
        private ILogger Logger;
        /// <summary>
        /// Creates product with images and category in db.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OperationResult> CreateItemAsync(ProductDTO product, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            Store.AutoSaveChanges = false;

            await Store.CreateProductAsync(product, cancellationToken);

            if (!product.CategoryName.IsNullOrEmpty())
            {
                //add prod to cat
            }

            if (!product.Images.IsNullOrEmpty())
            {
                //add images
            }

            Store.AutoSaveChanges = true;

            await Store.SaveChanges(cancellationToken);
            return OperationResult.Success();
        }

        /// <summary>Throws if this class has been disposed.</summary>
        protected void ThrowIfDisposed()
        {
            if (this._disposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        #region IDisposable

        private bool _disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
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

