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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Castle.Core.Internal;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Website.Data.EF.Models;
using Website.Service.DTO;
using Website.Service.Enums;
using Website.Service.Infrastructure;
using Website.Service.Interfaces;

namespace Website.Service.Stores
{
    /// <summary>
    /// Represents a new instance of a persistence store for the specified types.
    /// </summary>
    public class ShopStore : IShopStore<ProductDto, ProductImageDto, CategoryDto, OrderDto>
    {
        /// <summary>
        /// Constructs a new instance of CustomProductStoreBase".
        /// </summary>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="mapper">The <see cref="AutoMapper.Mapper"/>.</param>
        /// <param name="describer">The <see cref="OperationErrorDescriber"/>.</param>
        public ShopStore(DbContext dbContext, IMapper mapper, IHostingEnvironment environment, ILogger<ShopStore> logger, OperationErrorDescriber describer = null)
        {
            ErrorDescriber = describer ?? new OperationErrorDescriber();
            Context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _hostingEnvironment = environment;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public DbContext Context { get; private set; }
        public OperationErrorDescriber ErrorDescriber { get; set; }

        private bool _disposed;
        private readonly IMapper _mapper;
        private readonly ILogger<ShopStore> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private DbSet<Product> ProductsSet => Context.Set<Product>();
        private DbSet<Category> CategoriesSet => Context.Set<Category>();
        private DbSet<ProductImage> ImagesSet => Context.Set<ProductImage>();
        private DbSet<DescriptionGroup> DescGroupsSet => Context.Set<DescriptionGroup>();
        private DbSet<Description> DescriptionsSet => Context.Set<Description>();

        private const string ImagesSavePath = "images\\items";
        private const string ImagesSaveFormat = "jpg";

        /// <summary>
        /// Gets or sets a flag indicating if changes should be persisted after CreateAsync, UpdateAsync and DeleteAsync are called.
        /// </summary>
        /// <value>
        /// True if changes should be automatically persisted, otherwise false.
        /// </value>
        private bool AutoSaveChanges { get; set; } = true;

        /// <summary>Saves the current store.</summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public Task SaveChanges(CancellationToken cancellationToken)
        {
            return AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.CompletedTask;
        }

        #region CRUD product

        public async Task<OperationResult> CreateProductAsync(ProductDto product, CancellationToken cancellationToken = default(CancellationToken))
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
            catch (DbUpdateConcurrencyException)
            {
                return OperationResult.Failure(ErrorDescriber.ConcurrencyFailure());
            }

            var result = await SaveImagesAsync(product, cancellationToken);
            if (!result.Succeeded)
                return result;
            return OperationResult.Success();
        }

        public async Task<ProductDto> FindProductByIdAsync(int productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var product = await ProductsSet
                .Where(x => x.Id == productId)
                .Include(x => x.Images)
                .Include(x => x.Descriptions)
                .Include(x => x.ProductCategory)
                .ThenInclude(x => x.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return null;
            }
            var dto = _mapper.Map<ProductDto>(product);

            //images
            dto.Images = ConvertDbImageToDto(product.Images);

            foreach (var productToCategory in product.ProductCategory)
            {
                var cat = _mapper.Map<CategoryDto>(productToCategory.Category);
                dto.Categories.Add(cat);
            }
            return dto;
        }

        public async Task<ProductDto> FindProductByNameAsync(string productName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var product = await ProductsSet
                .Where(x => String.Equals(x.Name, productName, StringComparison.InvariantCultureIgnoreCase))
                .Include(x => x.Images)
                .Include(x => x.Descriptions)
                .Include(x => x.ProductCategory)
                .ThenInclude(x => x.Category)
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return null;
            }
            var dto = _mapper.Map<ProductDto>(product);

            //images
            dto.Images = ConvertDbImageToDto(product.Images);

            foreach (var productToCategory in product.ProductCategory)
            {
                var cat = _mapper.Map<CategoryDto>(productToCategory.Category);
                dto.Categories.Add(cat);
            }
            return dto;
        }

        public async Task<OperationResult> UpdateProductAsync(ProductDto product, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var dbProduct = _mapper.Map<Product>(product);

            //dont need if asnotracking
            // 
            //var local = ProductsSet
            //    .Local
            //    .FirstOrDefault(entry => entry.Id.Equals(product.Id));
            //// check if local is not null 
            //if (local != null) 
            //{
            //    // detach or remap
            //    Context.Entry(local).State = EntityState.Detached;
            //}
            //

            ProductsSet.Update(dbProduct);

            try
            {
                await SaveChanges(cancellationToken);
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

        public async Task<OperationResult> DeleteProductAsync(ProductDto product, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var entry = await ProductsSet.FindAsync(new[] { product.Id }, cancellationToken);
            if (entry != null)
            {
                ProductsSet.Remove(entry);
                try
                {
                    await SaveChanges(cancellationToken);
                }
                catch (DbUpdateException)
                {
                    return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
                }
            }
            return OperationResult.Success();
        }

        #endregion

        #region CRUD category

        public async Task<OperationResult> CreateCategoryAsync(CategoryDto category, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var dbCategory = _mapper.Map<Category>(category);
            CategoriesSet.Add(dbCategory);
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

        public async Task<CategoryDto> FindCategoryByNameAsync(string categoryName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (categoryName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(categoryName));

            var category = await CategoriesSet
                .Where(x => String.Equals(x.Name, categoryName, StringComparison.CurrentCultureIgnoreCase))
                .FirstOrDefaultAsync(cancellationToken);
            if (category == null)
                return null;
            var dbCategory = _mapper.Map<CategoryDto>(category);
            return dbCategory;
        }

        public async Task<OperationResult> UpdateCategoryAsync(CategoryDto category, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var dbCategory = _mapper.Map<Category>(category);
            CategoriesSet.Update(dbCategory);
            try
            {
                await SaveChanges(cancellationToken);
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

        public async Task<OperationResult> DeleteCategoryAsync(CategoryDto category, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var entry = await CategoriesSet.FindAsync(new[] { category.Id }, cancellationToken);
            if (entry != null)
            {
                CategoriesSet.Remove(entry);
                try
                {
                    await SaveChanges(cancellationToken);
                }
                catch (DbUpdateException)
                {
                    return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
                }
            }
            return OperationResult.Success();
        }

        #endregion

        #region images 

        public async Task<OperationResult> SaveImagesAsync(ProductDto product, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (product.Images.IsNullOrEmpty())
                return OperationResult.Success();

            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var imagesToSaveOnDisk = SaveImagesToContext(product, cancellationToken);
                    await SaveChanges(cancellationToken);
                    ProcessImagesOnDisk(imagesToSaveOnDisk);
                    transaction.Commit();
                }
                catch (DbUpdateException e)
                {
                    return OperationResult.Failure(ErrorDescriber.DbUpdateFailure());
                }
                catch (IOException e)
                {
                    return OperationResult.Failure(ErrorDescriber.DiskIOError());
                }
            }

            return OperationResult.Success();
        }

        private ImagesToDisk SaveImagesToContext(ProductDto product, CancellationToken cancellationToken)
        {
            var productId = product.Id;
            var imagesToDisk = new ImagesToDisk();
            foreach (var image in product.Images) //TODO tests 
            {
                var dbImage = new ProductImage
                {
                    ProductId = productId,
                    Path = ImagesSavePath.Replace("\\", "/"),
                    Primary = image.Primary
                };

                switch (image.DtoState)
                {
                    case DtoState.Added:
                        {
                            var fileFormat = ImagesSaveFormat.ToString().ToLower();
                            GetImageAvailableFileName(product.Id, fileFormat, out string imageName, out string imageThumbName, imagesToDisk.ImagesToSave);
                            imagesToDisk.ImagesToSave.Add(imageName, image.Bitmap);
                            dbImage.Name = imageName;
                            dbImage.ThumbName = imageThumbName;
                            dbImage.Format = fileFormat;
                            ImagesSet.Add(dbImage);
                            break;
                        }
                    case DtoState.Deleted:
                        {
                            if (!image.Id.HasValue)
                                break;
                            dbImage.Id = image.Id.Value;
                            if (!ImagesSet.Any(x => x.Id == image.Id))
                                break; //nothing to remove from db due to concurrency or smth
                            imagesToDisk.ImagesAndThumbsToDelete.Add((image.Path, image.ThumbPath));
                            ImagesSet.Remove(dbImage);
                            break;
                        }
                    case DtoState.Modified: //not used yet
                    default:
                        {
                            if (!image.Id.HasValue)
                                break;
                            dbImage.Id = image.Id.Value;
                            ImagesSet.Attach(dbImage);
                            Context.Entry(dbImage).Property(x => x.Primary).IsModified = true;
                            break;
                        }
                }
            }
            return imagesToDisk;
        }

        private void GetImageAvailableFileName(int productId, string fileFormat, out string imageName, out string imageThumbName,
            Dictionary<string, Bitmap> pendingAdditions)
        {
            var path = Path.Combine(_hostingEnvironment.WebRootPath, ImagesSavePath);
            int counter = 0;
            string imagePath;
            var files = Directory.GetFiles(path, productId + "_*");
            do
            {
                imageName = $"{productId}_{++counter}";
                imagePath = $"{path}\\{imageName}.{fileFormat}";
            } while (files.Contains(imagePath) || pendingAdditions.ContainsKey(imageName));

            imageThumbName = $"{productId}_{counter}_s";
        }

        private void ProcessImagesOnDisk(ImagesToDisk images)
        {

            //save
            foreach (var kvp in images.ImagesToSave)
            {
                var path = Path.Combine(_hostingEnvironment.WebRootPath, ImagesSavePath);
                var bitmap = kvp.Value;
                var name = kvp.Key;
                var fileFormat = ImagesSaveFormat.ToString().ToLower();
                var imagePath = $"{path}\\{name}.{fileFormat}";
                var imageThumbPath = $"{path}\\{name}_s.{fileFormat}";
                Bitmap thumbImage = StoreHelpers.ScaleImage(bitmap, 150, 150);

                var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                var encParams = new EncoderParameters() { Param = new[] { new EncoderParameter(Encoder.Quality, 70L) } };
                bitmap.Save(imagePath, encoder, encParams);
                thumbImage.Save(imageThumbPath, encoder, encParams);
            }
            //delete
            foreach (var imagesTuple in images.ImagesAndThumbsToDelete)
            {
                var path = _hostingEnvironment.WebRootPath;
                var imagePath = path + imagesTuple.Item1.Replace("/", "\\");
                var imageThumbPath = path + imagesTuple.Item2.Replace("/", "\\");
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                if (System.IO.File.Exists(imageThumbPath))
                {
                    System.IO.File.Delete(imageThumbPath);
                }
            }
        }

        private EncoderParameters GetEncoderParameters()
        {
            throw new NotImplementedException();
        }

        [Obsolete("not implemented")]
        public async Task<OperationResult> LoadImagesAsync(ProductDto product, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
            //cancellationToken.ThrowIfCancellationRequested();
            //ThrowIfDisposed();
            //if (product == null)
            //    throw new ArgumentNullException(nameof(product));

            //var dbImages = await ImagesSet.Where(x => x.ProductId == product.Id).ToListAsync(cancellationToken);
            //return dbImages.Any() ? null : ConvertDbImageToDto(dbImages);
        }

        #endregion 

        #region Descriptions

        public async Task<OperationResult> LoadProductDescriptions(ProductDto product, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var descs = await DescriptionsSet //get descriptions
                .Where(x => x.ProductId == product.Id)
                .Include(x => x.DescriptionGroupNavigation)
                .ToListAsync(cancellationToken);

            var descGroups = descs //get flattened description groups from description nav property
                .Select(desc => desc.DescriptionGroupNavigation)
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .Select(x => new DescriptionGroupDto() //convert description groups to dto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                })
                .OrderBy(x => x.Name != "Общие характеристики") //ordering
                .ThenBy(x => x.Name)
                .ToList();

            var descItems = descs //convert descriptions to dto items
                .OrderBy(x => x.Name) //ordering
                .Select(x => new DescriptionItemDto()
                {
                    Id = x.Id,
                    GroupId = x.DescriptionGroupId.GetValueOrDefault(),
                    Name = x.Name,
                    Value = x.Value
                })
                .ToList();

            //add descriptions to their respective groups
            foreach (var descGroup in descGroups)
            {
                foreach (var descItem in descItems)
                {
                    if (descItem.GroupId == descGroup.Id)
                    {
                        descGroup.Items.Add(descItem);
                    }
                }
            }
            product.Descriptions = descGroups;
            return OperationResult.Success();
        }

        #endregion

        private List<ProductImageDto> ConvertDbImageToDto(ICollection<ProductImage> productImages)
        {
            if (productImages == null)
                throw new ArgumentNullException(nameof(productImages));

            var images = productImages
                .Select(x => new ProductImageDto()
                {
                    Id = x.Id,
                    Path = $"/{x.Path}/{x.Name}.{x.Format}",
                    ThumbPath = $"/{x.Path}/{x.ThumbName}.{x.Format}",
                    Primary = x.Primary
                })
                .OrderBy(x => !x.Primary)
                .ToList();
            return images;
        }

        public async Task<SortPageResult<ProductDto>> SortFilterPageResultAsync(ItemTypeSelector types, string searchString, string sortPropName, int currPage,
            int countPerPage, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            if (sortPropName == null) throw new ArgumentNullException(nameof(sortPropName));
            if (countPerPage < 0) throw new ArgumentOutOfRangeException(nameof(countPerPage));
            if (currPage < 0) throw new ArgumentOutOfRangeException(nameof(currPage));
            if (!Enum.IsDefined(typeof(ItemTypeSelector), types))
                throw new InvalidEnumArgumentException(nameof(ItemTypeSelector), (int)types, typeof(ItemTypeSelector));

            IQueryable<Product> prodQuery = ProductsSet.AsQueryable();

            FilterProductsTypeQuery(types, ref prodQuery);
            SearchProductsQuery(searchString, ref prodQuery);
            OrderProductsQuery(sortPropName, ref prodQuery);
            PaginateProductsQuery(currPage, countPerPage, ref prodQuery);

            int totalProductsN = await prodQuery.CountAsync(cancellationToken);
            var productsDto = await prodQuery.ProjectTo<ProductDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return new SortPageResult<ProductDto> { FilteredData = productsDto, TotalN = totalProductsN };
        }

        private void PaginateProductsQuery(int currPage, int countPerPage, ref IQueryable<Product> prodQuery)
        {
            int skip = (currPage - 1) * countPerPage;
            int take = countPerPage;
            prodQuery = prodQuery.Skip(skip).Take(take);
        }

        private void FilterProductsTypeQuery(ItemTypeSelector types, ref IQueryable<Product> productQuery)
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

            var check = StoreHelpers.CheckIfPropertyExists(sortPropName, typeof(ProductDto));
            if (!check.Result)
                throw new ArgumentException(nameof(sortPropName)); //or set to default

            Expression<Func<Product, object>> property = p => EF.Property<object>(p, sortPropName);

            if (descending)
                prodQuery = prodQuery.OrderByDescending(property);
            else
                prodQuery = prodQuery.OrderBy(property);
        }


        private async Task<OperationResult> AddProductToCategory(ProductDto product, string categoryName, CancellationToken cancellationToken = default(CancellationToken))
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

        private void ThrowIfDisposed()
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