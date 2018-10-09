using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Core.Internal;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Website.Data.EF.Models;
using Website.Service.DTO;
using Website.Service.Infrastructure;
using Website.Service.Interfaces;

namespace Website.Service.Stores
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a new instance of a persistence store for products, using the default implementation.
    /// </summary>
    public class CustomShopStore : CustomShopStoreBase<ProductDTO, Product, Category>
    {
        public CustomShopStore(DbContext context, IMapper mapper, StoreErrorDescriber describer = null)
            : base(context, mapper, describer)
        {
        }
    }

    /// <summary>
    /// Represents a new instance of a persistence store for the specified products types.
    /// </summary>
    /// <typeparam name="TDtoProduct">The type representing a dto product.</typeparam>
    /// <typeparam name="TDbProduct">The type representing a product in database.</typeparam>
    public class CustomShopStoreBase<TDtoProduct, TDbProduct, TDbCategory>
        where TDtoProduct : ProductDTO
        where TDbProduct : Product
        where TDbCategory : Category, new()

    {
        /// <summary>
        /// Constructs a new instance of CustomProductStoreBase".
        /// </summary>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="mapper">The <see cref="AutoMapper.Mapper"/>.</param>
        /// <param name="describer">The <see cref="StoreErrorDescriber"/>.</param>
        public CustomShopStoreBase(DbContext dbContext, IMapper mapper, StoreErrorDescriber describer = null)
        {
            ErrorDescriber = describer ?? new StoreErrorDescriber();
            Context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public DbContext Context { get; private set; }
        public StoreErrorDescriber ErrorDescriber { get; set; }

        private bool _disposed;
        protected readonly IMapper _mapper;
        private DbSet<TDbProduct> ProductsSet => Context.Set<TDbProduct>();
        private DbSet<TDbCategory> CategoriesSet => Context.Set<TDbCategory>();

        /// <summary>
        /// Gets or sets a flag indicating if changes should be persisted after CreateAsync, UpdateAsync and DeleteAsync are called.
        /// </summary>
        /// <value>
        /// True if changes should be automatically persisted, otherwise false.
        /// </value>
        public bool AutoSaveChanges { get; set; } = true;

        /// <summary>Saves the current store.</summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public Task SaveChanges(CancellationToken cancellationToken)
        {
            return AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.CompletedTask;
        }


        //TODO move to manager
        /// <summary>
        /// Creates product with images and category in db.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> CreateItemAsync(TDtoProduct product, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            AutoSaveChanges = false;

            await CreateProductAsync(product, cancellationToken);

            if (!product.CategoryName.IsNullOrEmpty())
            {
                //add prod to cat
            }

            if (!product.Images.IsNullOrEmpty())
            {
                //add images
            }

            AutoSaveChanges = true;

            await SaveChanges(cancellationToken);
            return OperationResult.Success();
        }

        #region CRUD product

        private async Task<OperationResult> CreateProductAsync(TDtoProduct product, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var dbProduct = _mapper.Map<TDbProduct>(product);
            ProductsSet.Add(dbProduct);
            await SaveChanges(cancellationToken);
            return OperationResult.Success();
        }

        public virtual async Task<TDtoProduct> FindProductByIdAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (productId.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(productId));
            }

            var product = await ProductsSet.FindAsync(new object[] { productId }, cancellationToken);
            return product == null ? null : _mapper.Map<TDtoProduct>(product);
        }

        public virtual async Task<OperationResult> UpdateProductAsync(TDtoProduct product, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var dbProduct = _mapper.Map<TDbProduct>(product);
            ProductsSet.Update(dbProduct);
            await SaveChanges(cancellationToken);
            return OperationResult.Success();
        }

        public virtual async Task<OperationResult> RemoveProductAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (productId.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(productId));
            }

            var entry = await ProductsSet.FindAsync(new object[] { productId }, cancellationToken);
            if (entry != null)
            {
                ProductsSet.Remove(entry);
                await SaveChanges(cancellationToken);
            }
            return OperationResult.Success();
        }

        #endregion

        #region CRUD category

        private async Task<OperationResult> CreateCategoryAsync(string categoryName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (categoryName.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(categoryName));
            }

            var category = new TDbCategory() { Name = categoryName };
            CategoriesSet.Add(category);
            await SaveChanges(cancellationToken);
            return OperationResult.Success();
        }

        public virtual async Task<bool> FindCategoryByNameAsync(string categoryName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (categoryName.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(categoryName));
            }

            var category = await CategoriesSet.Where(x => String.Equals(x.Name, categoryName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync(cancellationToken);
            return category != null;
        }

        //public virtual async Task<OperationResult> UpdateCategoryAsync(TDtoProduct product, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    cancellationToken.ThrowIfCancellationRequested();
        //    ThrowIfDisposed();
        //    if (product == null)
        //    {
        //        throw new ArgumentNullException(nameof(product));
        //    }

        //    var dbProduct = _mapper.Map<TDbProduct>(product);
        //    ProductsSet.Update(dbProduct);
        //    await SaveChanges(cancellationToken);
        //    return OperationResult.Success();
        //}

        public virtual async Task<OperationResult> RemoveCategoryAsync(string categoryName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (categoryName.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(categoryName));
            }

            var entry = await CategoriesSet.Where(x => String.Equals(x.Name, categoryName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync(cancellationToken);
            if (entry != null)
            {
                CategoriesSet.Remove(entry);
                await SaveChanges(cancellationToken);
            }
            return OperationResult.Success();
        }

        #endregion

        #region CRUD images

        //TODO

        #endregion

        ///// <summary>
        ///// Creates the specified <paramref name="user"/> in the user store.
        ///// </summary>
        ///// <param name="user">The user to create.</param>
        ///// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        ///// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.</returns>
        //public virtual async Task<IdentityResult> CreateAsync(TDtoUser user, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    cancellationToken.ThrowIfCancellationRequested();
        //    ThrowIfDisposed();
        //    if (user == null)
        //    {
        //        throw new ArgumentNullException(nameof(user));
        //    }
        //    var dbUser = _mapper.Map<TDbUser>(user);
        //    Context.Add(dbUser);
        //    await SaveChanges(cancellationToken);
        //    return IdentityResult.Success;
        //}

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Dispose the store
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
            //dbcontext managed by DI
        }
    }
}