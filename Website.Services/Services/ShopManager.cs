using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Website.Core.Enums;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Repositories;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;
using Website.Services.Infrastructure;

namespace Website.Services.Services
{
    public class ShopManager : IShopManager, IDisposable
    {
        public ShopManager(
            IShopRepository<Product, Image, ImageBinData, Category, DescriptionGroup, DescriptionGroupItem,
                Description, Order> repository,
            ILogger<ShopManager> logger,
            IHttpContextAccessor context,
            IOptions<ShopManagerOptions> optionsAccessor,
            IEnumerable<IShopValidator<Product>> prodValidators,
            IEnumerable<IShopValidator<Image>> imgValidators,
            IEnumerable<IShopValidator<Category>> catValidators,
            IEnumerable<IShopValidator<DescriptionGroup>> descGroupValidators,
            IEnumerable<IShopValidator<Description>> descValidators,
            IEnumerable<IShopValidator<Order>> orderValidators,
            IShopImageTransformer<Image> imgTransformer,
            OperationErrorDescriber errorDescriber = null)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ErrorDescriber = errorDescriber ?? new OperationErrorDescriber();
            Options = optionsAccessor.Value ?? new ShopManagerOptions();
            CancellationToken = context?.HttpContext?.RequestAborted ?? CancellationToken.None;

            //TODO create single validator class
            foreach (var validator in prodValidators)
            {
                ProductValidators.Add(validator);
            }

            foreach (var validator in imgValidators)
            {
                ImageValidators.Add(validator);
            }

            foreach (var validator in catValidators)
            {
                CategoryValidators.Add(validator);
            }

            foreach (var validator in descGroupValidators)
            {
                DescriptionGroupValidators.Add(validator);
            }

            foreach (var validator in descValidators)
            {
                DescriptionValidators.Add(validator);
            }

            foreach (var validator in orderValidators)
            {
                OrderValidators.Add(validator);
            }

            ImageTransformer = imgTransformer ??
                               new ShopImageTransformer(optionsAccessor);
        }

        private readonly IShopRepository<Product, Image, ImageBinData, Category, DescriptionGroup, DescriptionGroupItem,
            Description, Order> _repository;

        public OperationErrorDescriber ErrorDescriber { get; }
        private readonly ILogger<ShopManager> _logger;
        public ShopManagerOptions Options { get; }

        public IList<IShopValidator<Product>> ProductValidators { get; } =
            new List<IShopValidator<Product>>();

        public IList<IShopValidator<Image>> ImageValidators { get; } =
            new List<IShopValidator<Image>>();

        public IList<IShopValidator<Category>> CategoryValidators { get; } =
            new List<IShopValidator<Category>>();

        public IList<IShopValidator<DescriptionGroup>> DescriptionGroupValidators { get; } =
            new List<IShopValidator<DescriptionGroup>>();

        public IList<IShopValidator<Description>> DescriptionValidators { get; } =
            new List<IShopValidator<Description>>();

        public IList<IShopValidator<Order>> OrderValidators { get; } =
            new List<IShopValidator<Order>>();

        public IShopImageTransformer<Image> ImageTransformer { get; }
        protected CancellationToken CancellationToken { get; }

        public async Task<OperationResult> CreateProductAsync(
            Product product,
            IEnumerable<Image> images,
            IEnumerable<int> categoryIds,
            IEnumerable<Description> descriptions)
        {
            ThrowIfDisposed();
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (images == null) images = Enumerable.Empty<Image>();
            if (categoryIds == null) categoryIds = Enumerable.Empty<int>();
            if (descriptions == null) descriptions = Enumerable.Empty<Description>();


            var result = await Validate(new[] {product}, ProductValidators);
            if (!result.Succeeded)
            {
                return result;
            }

            var imagesArray = images as Image[] ?? images.ToArray();
            result = await Validate(imagesArray, ImageValidators);
            if (!result.Succeeded)
            {
                return result;
            }

            var descsArray = descriptions as Description[] ?? descriptions.ToArray();
            result = await Validate(descsArray, DescriptionValidators);
            if (!result.Succeeded)
            {
                return result;
            }

            TransformImages(imagesArray);

            using (var tran = _repository.BeginTransaction())
            {
                result = await _repository.CreateProductAsync(product, CancellationToken);
                if (!result.Succeeded)
                {
                    return result;
                }

                if (!categoryIds.IsNullOrEmpty())
                {
                    result = await _repository.AddProductCategoriesAsync(product.Id, categoryIds,
                        CancellationToken);
                    if (!result.Succeeded)
                    {
                        return result;
                    }
                }

                if (!descsArray.IsNullOrEmpty())
                {
                    result = await _repository.CreateProductDescriptions(product.Id, descsArray, CancellationToken);
                    if (!result.Succeeded)
                    {
                        return result;
                    }
                }
                
                if (!imagesArray.IsNullOrEmpty())
                {
                    result = await _repository.CreateImagesAsync(product.Id, imagesArray, CancellationToken);
                    if (!result.Succeeded)
                    {
                        return result;
                    }
                }

                tran.Commit();
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> UpdateProductAsync(
            Product product,
            IEnumerable<Description> descriptionsToUpdate,
            IEnumerable<int> categoriesToUpdate,
            IEnumerable<Image> imgToUpdate)
        {
            ThrowIfDisposed();
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (descriptionsToUpdate == null) descriptionsToUpdate = Enumerable.Empty<Description>();
            if (categoriesToUpdate == null) categoriesToUpdate = Enumerable.Empty<int>();
            if (imgToUpdate == null) imgToUpdate = Enumerable.Empty<Image>();

            if (product.Id == 0)
            {
                return OperationResult.Failure(ErrorDescriber.InvalidProductId());
            }

            var result = await Validate(new[] {product}, ProductValidators);

            if (!result.Succeeded)
            {
                return result;
            }

            var descsToUpdateArray = descriptionsToUpdate as Description[] ?? descriptionsToUpdate.ToArray();
            result = await Validate(descsToUpdateArray, DescriptionValidators);

            if (!result.Succeeded)
            {
                return result;
            }

            var imgArray = imgToUpdate as Image[] ?? imgToUpdate.ToArray();
            result = await Validate(imgArray, ImageValidators, x => x.Id <= 0);
            if (!result.Succeeded)
            {
                return result;
            }

            TransformImages(imgArray);

            using (var tran = _repository.BeginTransaction())
            {
                result = await _repository.UpdateProductAsync(product, CancellationToken);
                if (!result.Succeeded)
                {
                    return result;
                }

                if (!descsToUpdateArray.IsNullOrEmpty())
                {
                    result = await _repository.UpdateProductDescriptions(product.Id, descsToUpdateArray,
                        CancellationToken);
                    if (!result.Succeeded)
                    {
                        return result;
                    }
                }

                if (!categoriesToUpdate.IsNullOrEmpty())
                {
                    result = await _repository.UpdateProductCategoriesAsync(product.Id, categoriesToUpdate,
                        CancellationToken);
                    if (!result.Succeeded)
                    {
                        return result;
                    }
                }

                if (!imgArray.IsNullOrEmpty())
                {
                    result = await _repository.UpdateImagesAsync(product.Id, imgArray, CancellationToken);
                    if (!result.Succeeded)
                    {
                        return result;
                    }
                }

                tran.Commit();
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteProductAsync(int productId)
        {
            ThrowIfDisposed();
            if (productId <= 0)
                return OperationResult.Failure(ErrorDescriber.InvalidProductId());


            var product = await _repository.FindProductByIdAsync(productId,
                false, false, false, CancellationToken);
            if (product == null)
            {
                return OperationResult.Failure(ErrorDescriber.CannotDeleteActiveProduct());
            }

            if (product.Available)
            {
                return OperationResult.Failure(ErrorDescriber.CannotDeleteActiveProduct());
            }

            var result = await _repository.DeleteProductAsync(product.Id, CancellationToken);
            if (!result.Succeeded)
            {
                return OperationResult.Failure(ErrorDescriber.ErrorDeletingProduct());
            }

            return OperationResult.Success();
        }

        public async Task<Product> GetProductByIdAsync(int id, bool loadImages, bool loadDescriptions,
            bool loadCategories)
        {
            ThrowIfDisposed();
            var product =
                await _repository.FindProductByIdAsync(id, loadImages, loadDescriptions, loadCategories,
                    CancellationToken);
            return product;
        }

        public async Task<Product> GetProductByNameAsync(string name, bool loadImages, bool loadDescriptions,
            bool loadCategories)
        {
            ThrowIfDisposed();
            var product = await _repository.FindProductByNameAsync(name, loadImages, loadDescriptions, loadCategories,
                CancellationToken);
            return product;
        }

        public async Task<Product> GetProductByCodeAsync(int code, bool loadImages, bool loadDescriptions,
            bool loadCategories)
        {
            ThrowIfDisposed();
            var product = await _repository.FindProductByCodeAsync(code, loadImages, loadDescriptions, loadCategories,
                CancellationToken);
            return product;
        }

        public async Task<SortPageResult<Product>> GetSortFilterPageAsync(ItemTypeSelector types,
            string searchString, string sortPropName, int currPage, int countPerPage)
        {
            ThrowIfDisposed();
            if (sortPropName == null) throw new ArgumentNullException(nameof(sortPropName));
            if (countPerPage < 0) throw new ArgumentOutOfRangeException(nameof(countPerPage));
            if (currPage < 0) throw new ArgumentOutOfRangeException(nameof(currPage));
            if (!Enum.IsDefined(typeof(ItemTypeSelector), types))
                throw new InvalidEnumArgumentException(nameof(ItemTypeSelector), (int) types, typeof(ItemTypeSelector));

            IQueryable<Product> prodQuery = _repository.ProductsQueryable;

            FilterProductsTypeQuery(types, ref prodQuery);
            SearchProductsQuery(searchString, ref prodQuery);
            OrderProductsQuery(sortPropName, ref prodQuery);
            int totalProductsN = await prodQuery.CountAsync(CancellationToken);
            PaginateProductsQuery(currPage, countPerPage, ref prodQuery);
            var products = await prodQuery.ToListAsync(CancellationToken);

            return new SortPageResult<Product> {FilteredData = products, TotalN = totalProductsN};
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
                    productQuery = productQuery.Where(x => x.Available);
                    break;
                case ItemTypeSelector.Disabled:
                    productQuery = productQuery.Where(x => !x.Available);
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

            var check = ServiceHelpers.CheckIfPropertyExists(sortPropName, typeof(Product));
            if (!check.Result)
                throw new ArgumentException(nameof(sortPropName)); //or set to default

            Expression<Func<Product, object>> property = p =>
                Microsoft.EntityFrameworkCore.EF.Property<object>(p, sortPropName);

            if (descending)
                prodQuery = prodQuery.OrderByDescending(property);
            else
                prodQuery = prodQuery.OrderBy(property);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            ThrowIfDisposed();
            return await _repository.GetAllCategoriesAsync(CancellationToken);
        }

        public async Task<IEnumerable<(Category, int)>> GetAllCategoriesWithProductCountAsync()
        {
            ThrowIfDisposed();
            return (await _repository.CategoriesQueryable
                    .Select(x => new {category = x, count = x.ProductCategory.Count})
                    .ToListAsync(CancellationToken))
                .Select(x => (x.category, x.count));
        }

        public async Task<IEnumerable<DescriptionGroup>> GetAllDescriptionGroupsAsync()
        {
            ThrowIfDisposed();
            return await _repository.GetDescriptionGroupsAsync(CancellationToken);
        }

        public async Task<IEnumerable<DescriptionGroupItem>> GetDescriptionItemsAsync(int groupId)
        {
            ThrowIfDisposed();
            return await _repository.GetDescriptionGroupItemsAsync(groupId, CancellationToken);
        }

        public async Task<(byte[], string)> GetImageDataMimeAsync(int imageId, bool thumb = false)
        {
            ThrowIfDisposed();
            if (!thumb)
            {
                return (await _repository.ImageDataQueryable
                        .Where(x => x.ImageId == imageId)
                        .Select(x => new {x.FullData, x.ImageInfo.Mime})
                        .ToListAsync(CancellationToken))
                    .Select(x => (x.FullData, x.Mime))
                    .FirstOrDefault();
            }
            else
            {
                return (await _repository.ImageDataQueryable
                        .Where(x => x.ImageId == imageId)
                        .Select(x => new {x.ThumbData, x.ImageInfo.Mime})
                        .ToListAsync(CancellationToken))
                    .Select(x => (x.ThumbData, x.Mime))
                    .FirstOrDefault();
                ;
            }
        }

        private void TransformImages(IEnumerable<Image> images)
        {
            if (images == null)
                return;

            foreach (var image in images)
            {
                if (image.BinData == null)
                {
                    continue;
                }

                ImageTransformer.ProcessImage(image);
            }
        }

        private async Task<OperationResult> Validate<T>(
            IEnumerable<T> items, IEnumerable<IShopValidator<T>> validators,
            Func<T, bool> predicateBeforeValidation = null) where T : class
        {
            var errors = new List<OperationError>();
            var itemArray = items as T[] ?? items.ToArray();
            foreach (var validator in validators)
            {
                foreach (var item in itemArray)
                {
                    if (predicateBeforeValidation?.Invoke(item) ?? true)
                    {
                        var result = await validator.ValidateAsync(this, item);
                        if (!result.Succeeded)
                        {
                            errors.AddRange(result.Errors);
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {
                return OperationResult.Failure(errors.ToArray());
            }

            return OperationResult.Success();
        }

        public static IEnumerable<T> Concatenate<T>(params IEnumerable<T>[] lists)
        {
            return lists.SelectMany(x => x);
        }

        private void ThrowIfDisposed()
        {
            if (this._disposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        #region IDisposable

        private bool _disposed = false;

        public void Dispose()
        {
            if (this._disposed)
                return;
            this._repository?.Dispose();
            _disposed = true;
        }

        #endregion
    }
}