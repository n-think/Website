﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Castle.Core.Internal;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Website.Data.EF.Models;
using Website.Service.DTO;
using Website.Service.Enums;
using Website.Service.Infrastructure;
using Website.Service.Interfaces;

namespace Website.Service.Stores
{


    /// <inheritdoc />
    /// <summary>
    /// Represents a new instance of a persistence store for products, using the default implementation.
    /// </summary>
    public class CustomShopStore : ShopStoreBase<ProductDTO, Product, Category>
    {
        public CustomShopStore(DbContext context, IMapper mapper, IHostingEnvironment environment, StoreErrorDescriber describer = null)
            : base(context, mapper, describer)
        {
        }
    }

    /// <summary>
    /// Represents a new instance of a persistence store for the specified types.
    /// </summary>
    /// <typeparam name="TDtoProduct">The type representing a dto product.</typeparam>
    /// <typeparam name="TDbProduct">The type representing a product in database.</typeparam>
    public class ShopStoreBase<TDtoProduct, TDbProduct, TDbCategory> : IShopStore<TDtoProduct, TDbProduct>, IDisposable
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
        public ShopStoreBase(DbContext dbContext, IMapper mapper, StoreErrorDescriber describer = null)
        {
            ErrorDescriber = describer ?? new StoreErrorDescriber();
            Context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        public DbContext Context { get; private set; }
        public StoreErrorDescriber ErrorDescriber { get; set; }

        private bool _disposed;
        private readonly IMapper _mapper;
        private readonly object _lock = new object();
        protected DbSet<TDbProduct> ProductsSet => Context.Set<TDbProduct>();
        protected DbSet<TDbCategory> CategoriesSet => Context.Set<TDbCategory>();

        private const string ImagesSavePath = "\\images\\products";
        /// <summary>
        /// Gets or sets a flag indicating if changes should be persisted after CreateAsync, UpdateAsync and DeleteAsync are called.
        /// </summary>
        /// <value>
        /// True if changes should be automatically persisted, otherwise false.
        /// </value>
        public bool AutoSaveChanges { get; set; } = true;

        public IQueryable<TDbProduct> ProductsQueryable { get => ProductsSet.AsQueryable(); }

        /// <summary>Saves the current store.</summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public Task SaveChanges(CancellationToken cancellationToken)
        {
            return AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.CompletedTask;
        }

        #region CRUD product

        public async Task<OperationResult> CreateProductAsync(TDtoProduct product, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var dbProduct = _mapper.Map<TDbProduct>(product);
            ProductsSet.Add(dbProduct);
            await SaveChanges(cancellationToken);
            return OperationResult.Success();
        }

        public async Task<TDtoProduct> FindProductByIdAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (productId.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(productId));

            var product = await ProductsSet.FindAsync(new object[] { productId }, cancellationToken);
            return product == null ? null : _mapper.Map<TDtoProduct>(product);
        }

        public async Task<OperationResult> UpdateProductAsync(TDtoProduct product, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var dbProduct = _mapper.Map<TDbProduct>(product);
            ProductsSet.Update(dbProduct);
            await SaveChanges(cancellationToken);
            return OperationResult.Success();
        }

        public async Task<OperationResult> RemoveProductAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (productId.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(productId));

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

        public async Task<OperationResult> CreateCategoryAsync(string categoryName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (categoryName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(categoryName));

            var category = new TDbCategory() { Name = categoryName };
            CategoriesSet.Add(category);
            await SaveChanges(cancellationToken);
            return OperationResult.Success();
        }

        public async Task<bool> FindCategoryByNameAsync(string categoryName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (categoryName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(categoryName));

            var category = await CategoriesSet.Where(x => String.Equals(x.Name, categoryName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync(cancellationToken);
            return category != null;
        }

        //public async Task<OperationResult> UpdateCategoryAsync(TDtoProduct product, CancellationToken cancellationToken = default(CancellationToken))
        //{
        ////    cancellationToken.ThrowIfCancellationRequested();
        ////    ThrowIfDisposed();
        ////    if (product == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(product));
        ////    }

        ////    var dbProduct = _mapper.Map<TDbProduct>(product);
        ////    ProductsSet.Update(dbProduct);
        ////    await SaveChanges(cancellationToken);
        ////    return OperationResult.Success();
        //}

        public async Task<OperationResult> RemoveCategoryAsync(string categoryName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (categoryName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(categoryName));

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

        public Task<OperationResult> SaveImage(Bitmap image, string savePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (savePath.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(savePath));
            if (Directory.Exists(savePath))
                throw new ArgumentException(nameof(savePath));

            image.Save(savePath);
            return Task.FromResult(OperationResult.Success());
        }

        public Task<OperationResult> RemoveImage(Bitmap image, string removePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (removePath.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(removePath));
            if (!Directory.Exists(removePath))
                throw new ArgumentException(nameof(removePath));

            File.Delete(removePath);
            return Task.FromResult(OperationResult.Success());
        }

        public void FilterProducstTypeQuery(ItemTypeSelector types, ref IQueryable<Product> productQuery)
        {
            ThrowIfDisposed();
            if (productQuery == null)
                throw new ArgumentNullException(nameof(productQuery));

            switch (types)
            {
                case ItemTypeSelector.Enabled:
                    productQuery = productQuery.Where(x => x.Enabled);
                    break;
                case ItemTypeSelector.Disabled:
                    productQuery = productQuery.Where(x => !x.Enabled);
                    break;
                default:
                    break;
            }
        }

        public void SearchProductsQuery(string searchString, ref IQueryable<Product> prodQuery)
        {
            this.ThrowIfDisposed();

            if (!String.IsNullOrEmpty(searchString))
            {
                prodQuery = prodQuery.Where(x =>
                    x.Name.Contains(searchString) || x.Code.ToString().Contains(searchString));
            }
        }

        public void OrderProductsQuery(string sortPropName, ref IQueryable<Product> prodQuery)
        {
            this.ThrowIfDisposed();
            if (sortPropName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(sortPropName));

            bool descending = false;
            if (sortPropName.EndsWith("_desc"))
            {
                sortPropName = sortPropName.Substring(0, sortPropName.Length - 5);
                descending = true;
            }

            var check = StoreHelpers.CheckIfPropertyExists(sortPropName, typeof(ProductDTO));
            if (!check.Result)
                throw new ArgumentNullException(nameof(sortPropName));

            Expression<Func<Product, object>> property = p => EF.Property<object>(p, sortPropName);

            if (descending)
                prodQuery = prodQuery.OrderByDescending(property);
            else
                prodQuery = prodQuery.OrderBy(property);
        }

        public async Task<int> CountQueryAsync(IQueryable<Product> Query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Query.CountAsync(cancellationToken);
        }

        public void SkipTakeQuery(int skipN, int takeN, ref IQueryable<Product> prodQuery)
        {
            prodQuery = prodQuery.Skip(skipN).Take(takeN);
        }

        public async Task<IEnumerable<ProductDTO>> ExecuteProductsQuery(IQueryable<Product> prodQuery, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await prodQuery.ProjectTo<ProductDTO>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
        }

        #endregion

        public async Task<OperationResult> AddProductToCategory(ProductDTO product, string categoryName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (categoryName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(categoryName));

            throw new NotImplementedException();

            return OperationResult.Success();
        }
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