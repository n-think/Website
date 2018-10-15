using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using NewArrayExpression = Castle.DynamicProxy.Generators.Emitters.SimpleAST.NewArrayExpression;

namespace Website.Service.Stores
{
    /// <summary>
    /// Represents a new instance of a persistence store for the specified types.
    /// </summary>
    public class ShopStore : IShopStore<ProductDTO, ProductImageDTO, OrderDTO>
    {
        /// <summary>
        /// Constructs a new instance of CustomProductStoreBase".
        /// </summary>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="mapper">The <see cref="AutoMapper.Mapper"/>.</param>
        /// <param name="describer">The <see cref="OperationErrorDescriber"/>.</param>
        public ShopStore(DbContext dbContext, IMapper mapper, IHostingEnvironment environment, OperationErrorDescriber describer = null)
        {
            ErrorDescriber = describer ?? new OperationErrorDescriber();
            Context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _hostingEnvironment = environment;

        }

        public DbContext Context { get; private set; }
        public OperationErrorDescriber ErrorDescriber { get; set; }

        private bool _disposed;
        private readonly IMapper _mapper;
        private readonly object _lock = new object();
        private readonly IHostingEnvironment _hostingEnvironment;
        private DbSet<Product> ProductsSet => Context.Set<Product>();
        private DbSet<Category> CategoriesSet => Context.Set<Category>();
        private DbSet<ProductImage> ImagesSet => Context.Set<ProductImage>();

        private const string ImagesSavePath = "images\\items";

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

        #region CRUD product

        public async Task<OperationResult> CreateProductAsync(ProductDTO product, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var dbProduct = _mapper.Map<Product>(product);
            ProductsSet.Add(dbProduct);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException e)
            {
                return OperationResult.Failure(ErrorDescriber.ConcurrencyFailure());
            }

            var result = await CreateImagesAsync(dbProduct.Id, product.Images, cancellationToken);
            if (!result.Succeeded)
                return result;
            return OperationResult.Success();
        }

        public async Task<ProductDTO> FindProductByIdAsync(int productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var product = await ProductsSet
                .Where(x => x.Id == productId)
                .Include(x => x.Images)
                .Include(x => x.Descriptions)
                .Include(x => x.ProductCategory)
                .ThenInclude(x => x.Category)
                .FirstAsync(cancellationToken);

            if (product == null)
            {
                return null;
            }
            var dto = _mapper.Map<ProductDTO>(product);

            //images
            dto.Images = ConvertDbImageToDto(product.Images);

            foreach (var productToCategory in product.ProductCategory)
            {
                var cat = _mapper.Map<CategoryDTO>(productToCategory.Category);
                dto.Categories.Add(cat);
            }
            return dto;
        }

        public async Task<OperationResult> UpdateProductAsync(ProductDTO product, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var dbProduct = _mapper.Map<Product>(product);

            ProductsSet.Update(dbProduct);
            await SaveChanges(cancellationToken);
            await UpdateImagesAsync(dbProduct.Id, product.Images, cancellationToken);

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteProductAsync(int productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

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

            var category = new Category() { Name = categoryName };
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


        private async Task<OperationResult> CreateImageAsync(int productId, Bitmap image,
            CancellationToken cancellationToken)
        {

            throw new NotImplementedException();
        }

        private async Task<OperationResult> CreateImagesAsync(int productId, ProductImageDTO[] images, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (images == null)
                throw new ArgumentNullException(nameof(images));

            int counter = 0;
            string type = "jpg";
            var pathToFiles = Path.Combine(_hostingEnvironment.WebRootPath, ImagesSavePath);

            foreach (var image in images) //TODO tests 
            {
                //TODO input orig - save normalized+thumb+orig
                //TODO ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                //TODO ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                //TODO ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                //TODO ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                if (image.Primary)
                {
                    string fileName = $"{productId}.{type}";
                    string savePath = Path.Combine(pathToFiles, fileName);
                    await CheckPathAndDeleteIfExists(savePath);
                    image.Bitmap.Save(savePath);

                    string dbPath = $"~/{ImagesSavePath.Replace("\\", "/")}/{fileName}";
                    var imageToAdd = new ProductImage() { Path = dbPath, ProductId = productId };
                    ImagesSet.Add(imageToAdd);
                }
                else
                {
                    string fileName = $"{productId}_{counter}.{type}";
                    string savePath = Path.Combine(pathToFiles, fileName);
                    await CheckPathAndDeleteIfExists(savePath);
                    image.Bitmap.Save(savePath);

                    string dbPath = $"~/{ImagesSavePath.Replace("\\", "/")}/{fileName}";
                    counter++;
                    var imageToAdd = new ProductImage() { Path = dbPath, ProductId = productId };
                    ImagesSet.Add(imageToAdd);
                }
            }

            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
            }
            return OperationResult.Success();
        }

        private Task CheckPathAndDeleteIfExists(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            return Task.CompletedTask;
        }

        private async Task<ProductImageDTO[]> FindImagesAsync(Product product, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var dbImages = await ImagesSet.Where(x => x.ProductId == product.Id).ToListAsync(cancellationToken);
            return dbImages.Any() ? null : ConvertDbImageToDto(dbImages);
        }

        private async Task<OperationResult> UpdateImagesAsync(int productId, ProductImageDTO[] images, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            throw new NotImplementedException();
            //DeleteImagesAsync();
            //CreateImagesAsync();
        }

        private async Task<OperationResult> DeleteImagesAsync(int productId, ProductImageDTO[] images, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            throw new NotImplementedException();
            return OperationResult.Success();
        }


        #endregion

        private ProductImageDTO[] ConvertDbImageToDto(ICollection<ProductImage> productImages)
        {
            if (productImages == null)
                throw new ArgumentNullException(nameof(productImages));

            var images = productImages
                .Select(x => new ProductImageDTO()
                {
                    Path = $"{x.Path}/{x.Name}.{x.Format}",
                    ThumbPath = $"{x.Path}/{x.ThumbName}.{x.Format}",
                    Primary = x.Primary
                })
                .OrderBy(x => !x.Primary)
                .ToArray();
            return images;
        }

        public async Task<SortPageResult<ProductDTO>> SortFilterPageResultAsync(ItemTypeSelector types, string searchString, string sortPropName, int currPage,
            int countPerPage, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            if (sortPropName == null) throw new ArgumentNullException(nameof(sortPropName));
            if (countPerPage < 0) throw new ArgumentOutOfRangeException(nameof(countPerPage));
            if (currPage < 0) throw new ArgumentOutOfRangeException(nameof(currPage));
            if (!Enum.IsDefined(typeof(ItemTypeSelector), types))
                throw new InvalidEnumArgumentException(nameof(ItemTypeSelector), (int)types, typeof(ItemTypeSelector));

            IQueryable<Product> prodQuery = ProductsSet.AsQueryable();

            FilterProducstTypeQuery(types, ref prodQuery);
            SearchProductsQuery(searchString, ref prodQuery);
            OrderProductsQuery(sortPropName, ref prodQuery);
            PaginateProductsQuery(currPage, countPerPage, ref prodQuery);

            int totalProductsN = await prodQuery.CountAsync(cancellationToken);
            var productsDto = await prodQuery.ProjectTo<ProductDTO>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return new SortPageResult<ProductDTO> { FilteredData = productsDto, TotalN = totalProductsN };
        }

        private void PaginateProductsQuery(int currPage, int countPerPage, ref IQueryable<Product> prodQuery)
        {
            int skip = (currPage - 1) * countPerPage;
            int take = countPerPage;
            prodQuery = prodQuery.Skip(skip).Take(take);
        }

        private void FilterProducstTypeQuery(ItemTypeSelector types, ref IQueryable<Product> productQuery)
        {
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

        private void SearchProductsQuery(string searchString, ref IQueryable<Product> prodQuery)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                prodQuery = prodQuery.Where(x =>
                    x.Name.Contains(searchString) || x.Code.ToString().Contains(searchString));
            }
        }

        private void OrderProductsQuery(string sortPropName, ref IQueryable<Product> prodQuery)
        {
            bool descending = false;
            if (sortPropName.EndsWith("_desc"))
            {
                sortPropName = sortPropName.Substring(0, sortPropName.Length - 5);
                descending = true;
            }

            var check = StoreHelpers.CheckIfPropertyExists(sortPropName, typeof(ProductDTO));
            if (!check.Result)
                throw new ArgumentException(nameof(sortPropName)); //or set to default

            Expression<Func<Product, object>> property = p => EF.Property<object>(p, sortPropName);

            if (descending)
                prodQuery = prodQuery.OrderByDescending(property);
            else
                prodQuery = prodQuery.OrderBy(property);
        }


        private async Task<OperationResult> AddProductToCategory(ProductDTO product, string categoryName, CancellationToken cancellationToken = default(CancellationToken))
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