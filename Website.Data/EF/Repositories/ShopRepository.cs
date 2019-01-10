using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Repositories;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;

namespace Website.Data.EF.Repositories
{
    public class ShopRepository : IShopRepository<Product, Image, Category, DescriptionGroup,
        Description, Order>
    {
        public ShopRepository(DbContext dbContext, IHostingEnvironment environment,
            ILogger<ShopRepository> logger, OperationErrorDescriber describer = null)
        {
            ErrorDescriber = describer ?? new OperationErrorDescriber();
            Context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _hostingEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private DbContext Context { get; set; }

        private OperationErrorDescriber ErrorDescriber { get; set; }

        private bool _disposed;

        private readonly ILogger<ShopRepository> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private DbSet<Product> ProductsSet => Context.Set<Product>();
        private DbSet<ProductToCategory> ProdToCatSet => Context.Set<ProductToCategory>();
        private DbSet<Category> CategoriesSet => Context.Set<Category>();
        private DbSet<Image> ImagesSet => Context.Set<Image>();
        private DbSet<DescriptionGroup> DescGroupsSet => Context.Set<DescriptionGroup>();
        private DbSet<Description> DescriptionsSet => Context.Set<Description>();
        public IQueryable<Product> ProductsQueryable => ProductsSet.AsQueryable();
        public IQueryable<Category> CategoriesQueryable => CategoriesSet.AsQueryable();
        public IQueryable<Image> ImagesQueryable => ImagesSet.AsQueryable();
        public IQueryable<Description> DescriptionsQueryable => DescriptionsSet.AsQueryable();
        public IQueryable<DescriptionGroup> DescriptionGroupsQueryable => DescGroupsSet.AsQueryable();

        public IDbContextTransaction BeginTransaction(IsolationLevel iLevel = IsolationLevel.Serializable) =>
            Context.Database.BeginTransaction(iLevel);

        public void JoinTransaction(IDbContextTransaction tran)
        {
            if (tran == null) throw new ArgumentNullException(nameof(tran));
            Context.Database.UseTransaction(tran.GetDbTransaction());
        }

        private async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return await Context.SaveChangesAsync(ct);
        }

        #region product

        public async Task<OperationResult> CreateProductAsync(Product product,
            CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            product.Created = DateTimeOffset.Now;
            product.Changed = product.Created;
            product.Id = 0; //reset id to default before adding
            ProductsSet.Attach(product);
            Context.Entry(product).State = EntityState.Added;
            try
            {
                await SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return OperationResult.Failure(ErrorDescriber.ConcurrencyFailure());
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> UpdateProductAsync(Product product,
            CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            product.Changed = DateTimeOffset.Now;
            ProductsSet.Attach(product);
            Context.Entry(product).State = EntityState.Modified;
            try
            {
                await Context.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return OperationResult.Failure(ErrorDescriber.ConcurrencyFailure());
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
            }

            return OperationResult.Success();
        }

        public async Task<Product> FindProductByIdAsync(int productId, bool loadImages,
            bool loadDescriptions, bool loadCategories,
            CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var query = ProductsSet
                .Where(x => x.Id == productId);

            return await GetProductFromQuery(query, loadImages, loadDescriptions, loadCategories, ct);
        }

        public async Task<Product> FindProductByNameAsync(string productName, bool loadImages,
            bool loadDescriptions, bool loadCategories,
            CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var query = ProductsSet
                .Where(x => String.Equals(x.Name, productName, StringComparison.InvariantCultureIgnoreCase));

            return await GetProductFromQuery(query, loadImages, loadDescriptions, loadCategories, ct);
        }

        public async Task<Product> FindProductByCodeAsync(int productCode, bool loadImages,
            bool loadDescriptions, bool loadCategories,
            CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var query = ProductsSet
                .Where(x => x.Code == productCode);

            return await GetProductFromQuery(query, loadImages, loadDescriptions, loadCategories, ct);
        }

        private async Task<Product> GetProductFromQuery(IQueryable<Product> query, bool loadImages,
            bool loadDescriptions, bool loadCategories, CancellationToken ct)
        {
            if (query == null)
                return null;

            if (loadImages)
            {
                query = query
                    .Include(x => x.Images);
            }

            if (loadCategories)
            {
                query = query
                    .Include(x => x.ProductToCategory)
                    .ThenInclude(x => x.Category);
            }

            if (loadDescriptions)
            {
                query = query
                    .Include(x => x.Descriptions)
                    .ThenInclude(x=>x.DescriptionGroup);
            }

            var product = await query
                //.AsNoTracking() //TODO test this
                .FirstOrDefaultAsync(ct);

            return product;
        }

        public async Task<OperationResult> DeleteProductAsync(Product product,
            CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var entry = await ProductsSet.FindAsync(new object[] {product.Id}, ct);

            if (entry == null)
            {
                return OperationResult.Failure(ErrorDescriber.EntityNotFound("Продукт"));
            }

            ProductsSet.Attach(product);
            Context.Entry(product).State = EntityState.Deleted;
            try
            {
                await SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return OperationResult.Failure(ErrorDescriber.ConcurrencyFailure());
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
            }

            return OperationResult.Success();
        }

        #endregion

        #region categories

        public async Task<OperationResult> CreateCategoryAsync(Category category,
            CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            CategoriesSet.Attach(category);
            Context.Entry(category).State = EntityState.Added;
            try
            {
                await SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
            }

            return OperationResult.Success();
        }

        public async Task<Category> FindCategoryByNameAsync(string categoryName,
            CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (categoryName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(categoryName));

            var category = await CategoriesSet
                .Where(x => String.Equals(x.Name, categoryName, StringComparison.CurrentCultureIgnoreCase))
                .FirstOrDefaultAsync(ct);

            return category;
        }

        public async Task<OperationResult> UpdateCategoryAsync(Category category,
            CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            CategoriesSet.Attach(category);
            Context.Entry(category).State = EntityState.Modified;
            try
            {
                await SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return OperationResult.Failure(ErrorDescriber.ConcurrencyFailure());
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteCategoryAsync(Category category,
            CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var entry = await CategoriesSet.FindAsync(new[] {category.Id}, ct);
            if (entry == null)
            {
                return OperationResult.Failure(ErrorDescriber.EntityNotFound("Категория"));
            }

            CategoriesSet.Attach(category);
            Context.Entry(category).State = EntityState.Deleted;
            try
            {
                await SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
            }

            return OperationResult.Success();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await CategoriesSet.ToListAsync(ct);
        }

        public async Task<OperationResult> AddProductCategoriesAsync(int productId,
            IEnumerable<ProductToCategory> categories,
            CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var productToCategories = categories as ProductToCategory[] ?? categories.ToArray();
            if (productToCategories.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(categories));

            var currentProdCats = await ProdToCatSet
                .Where(x => productId == x.ProductId)
                .ToListAsync(ct);

            foreach (var prodToCat in productToCategories)
            {
                if (!currentProdCats.Contains(prodToCat))
                {
                    ProdToCatSet.Attach(prodToCat);
                    Context.Entry(prodToCat).State = EntityState.Added;
                }
            }

            try
            {
                await SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return OperationResult.Failure(ErrorDescriber.ConcurrencyFailure());
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteProductCategoriesAsync(int productId,
            IEnumerable<ProductToCategory> categories,
            CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var productToCategories = categories as ProductToCategory[] ?? categories.ToArray();
            if (productToCategories.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(categories));

            var currentProdCats = await ProdToCatSet
                .Where(x => productId == x.ProductId)
                .ToListAsync(ct);

            foreach (var prodToCat in productToCategories)
            {
                if (!currentProdCats.Contains(prodToCat))
                {
                    ProdToCatSet.Attach(prodToCat);
                    Context.Entry(prodToCat).State = EntityState.Deleted;
                }
            }

            try
            {
                await SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return OperationResult.Failure(ErrorDescriber.ConcurrencyFailure());
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
            }

            return OperationResult.Success();
        }

        #endregion

        #region images 

        public async Task<OperationResult> CreateImagesAsync(int productId, IEnumerable<Image> images,
            CancellationToken ct = default(CancellationToken))
        {
            var productImages = images.ToList();
            foreach (var image in productImages)
            {
                if (image.BinData == null)
                {
                    return OperationResult.Failure(ErrorDescriber.InvalidModel());
                }
            }

            if (await ProductsSet.AnyAsync(x => x.Id == productId, ct))
            {
                return OperationResult.Failure(ErrorDescriber.EntityNotFound("Продукт"));
            }

            foreach (var image in productImages)
            {
                ImagesSet.Attach(image);
                Context.Entry(image).State = EntityState.Added;
                if (image.BinData != null)
                {
                    Context.Entry(image.BinData).State = EntityState.Added;
                }
            }

            try
            {
                await SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteImagesAsync(int productId, IEnumerable<Image> images,
            CancellationToken ct = default(CancellationToken))
        {
            var productImages = images.ToList();

            if (!await ProductsSet.AnyAsync(x => x.Id == productId, ct))
            {
                return OperationResult.Failure(ErrorDescriber.EntityNotFound("Продукт"));
            }

            foreach (var image in productImages)
            {
                ImagesSet.Attach(image);
                Context.Entry(image).State = EntityState.Deleted;
            }

            try
            {
                await SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
            }

            return OperationResult.Success();
        }

        #endregion

        #region Descriptions

        public async Task<OperationResult> CreateProductDescriptions(int productId,
            IEnumerable<Description> newDescriptions, CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var descriptions = newDescriptions as Description[] ?? newDescriptions.ToArray();
            if (descriptions.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(newDescriptions));

            var oldDescs = await DescriptionsSet
                .Where(x => x.ProductId == productId)
                .ToListAsync(ct);

            foreach (var desc in descriptions)
            {
                if (!oldDescs.Contains(desc))
                {
                    DescriptionsSet.Attach(desc);
                    Context.Entry(desc).State = EntityState.Added;
                }
            }

            try
            {
                await SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return OperationResult.Failure(ErrorDescriber.ConcurrencyFailure());
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteProductDescriptions(int productId,
            IEnumerable<Description> newDescriptions, CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var descriptions = newDescriptions as Description[] ?? newDescriptions.ToArray();
            if (descriptions.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(newDescriptions));

            var oldDescs = await DescriptionsSet
                .Where(x => x.ProductId == productId)
                .ToListAsync(ct);

            foreach (var desc in descriptions)
            {
                if (!oldDescs.Contains(desc))
                {
                    DescriptionsSet.Attach(desc);
                    Context.Entry(desc).State = EntityState.Deleted;
                }
            }

            try
            {
                await SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return OperationResult.Failure(ErrorDescriber.ConcurrencyFailure());
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
            }

            return OperationResult.Success();
        }

        public async Task<List<Description>> GetProductDescriptions(int productId,
            CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (await ProductsSet.AnyAsync(x => x.Id == productId, ct))
            {
                //TODO not sure what to throw here
                throw new ArgumentException(nameof(productId));
            }

            var descs = await DescriptionsSet
                .Where(x => x.ProductId == productId)
                .Include(x => x.DescriptionGroup)
                .ToListAsync(ct);

            return descs;
        }

        public async Task<IEnumerable<DescriptionGroup>> GetAllDescriptionGroupsAsync(
            CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await DescGroupsSet.ToListAsync(ct);
        }

        #endregion

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public void Dispose()
        {
            _disposed = true;
            //dbcontext managed by DI
        }
    }
}