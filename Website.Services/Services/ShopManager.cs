using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
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
            IShopRepository repository,
            ILogger<ShopManager> logger,
            IHttpContextAccessor context,
            IOptions<ShopManagerOptions> optionsAccessor,
            IEnumerable<IShopValidator<Product>> prodValidators,
            IEnumerable<IShopValidator<Image>> imgValidators,
            IEnumerable<IShopValidator<Category>> catValidators,
            IEnumerable<IShopValidator<DescriptionGroup>> descGroupValidators,
            IEnumerable<IShopValidator<DescriptionGroupItem>> descGroupItemValidators,
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

            foreach (var validator in descGroupItemValidators)
            {
                DescriptionGroupItemValidators.Add(validator);
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

        private readonly IShopRepository _repository;

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

        public IList<IShopValidator<DescriptionGroupItem>> DescriptionGroupItemValidators { get; } =
            new List<IShopValidator<DescriptionGroupItem>>();

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

        public async Task<IEnumerable<Product>> SearchProductsByName(string searchString, int count)
        {
            ThrowIfDisposed();
            if (searchString.IsNullOrEmpty())
                return Enumerable.Empty<Product>();
            return await _repository.ProductsQueryable
                .Where(x => x.Name.Contains(searchString))
                .Take(count)
                .Include(x=>x.Images)
                .ToListAsync(CancellationToken);
        }

        public async Task<SortPageResult<Product>> GetSortFilterPageAsync(ItemTypeSelector types, int currPage,
            int countPerPage, string searchString = null, string sortPropName = null, int[] categoryIds = null,
            int[] descGroupIds = null)
        {
            ThrowIfDisposed();
            if (countPerPage < 0) throw new ArgumentOutOfRangeException(nameof(countPerPage));
            if (currPage < 0) throw new ArgumentOutOfRangeException(nameof(currPage));
            if (!Enum.IsDefined(typeof(ItemTypeSelector), types))
                throw new InvalidEnumArgumentException(nameof(ItemTypeSelector), (int) types, typeof(ItemTypeSelector));

            var prodQuery = await FilterProductsQuery(_repository.ProductsQueryable, types, categoryIds, descGroupIds);
            SearchProductsQuery(searchString, ref prodQuery);
            OrderProductsQuery(sortPropName, ref prodQuery);
            var totalProductsN = await prodQuery.CountAsync(CancellationToken);
            PaginateProductsQuery(currPage, countPerPage, ref prodQuery);
            var products = await prodQuery
                .Include(x => x.Images)
                .ToListAsync(CancellationToken);

            return new SortPageResult<Product> {FilteredData = products, TotalN = totalProductsN};
        }

        private void PaginateProductsQuery(int currPage, int countPerPage, ref IQueryable<Product> prodQuery)
        {
            int skip = (currPage - 1) * countPerPage;
            int take = countPerPage;
            prodQuery = prodQuery.Skip(skip).Take(take);
        }

        private async Task<IQueryable<Product>> FilterProductsQuery(IQueryable<Product> productQuery,
            ItemTypeSelector types, int[] categoryIds, int[] descGroupIds)
        {
            //filter cats
            if (!categoryIds.IsNullOrEmpty())
            {
                productQuery = productQuery
                    .Where(x => x.ProductToCategory
                        .Any(z => categoryIds.Contains(z.CategoryId)));
            }

            if (!descGroupIds.IsNullOrEmpty())
            {
                //filter desc groups
                var productIdsInDescGroups = await _repository.DescriptionGroupsQueryable
                    .Where(x => descGroupIds.Contains(x.Id))
                    .SelectMany(x => x.DescriptionGroupItems)
                    .SelectMany(x => x.Descriptions)
                    .Select(x => x.ProductId)
                    .Distinct()
                    .ToArrayAsync(CancellationToken);

                productQuery = productQuery
                    .Where(x => productIdsInDescGroups.Contains(x.Id));
            }

            //filter types
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

            return productQuery;
        }

        private void SearchProductsQuery(string searchString, ref IQueryable<Product> prodQuery)
        {
            if (string.IsNullOrEmpty(searchString))
                return;

            prodQuery = prodQuery.Where(x =>
                x.Name.Contains(searchString) || x.Code.ToString().Contains(searchString));
        }

        private void OrderProductsQuery(string sortPropName, ref IQueryable<Product> prodQuery)
        {
            if (sortPropName.IsNullOrEmpty())
                return;

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

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            ThrowIfDisposed();
            return await _repository.FindCategoryByIdAsync(id, CancellationToken);
        }

        public async Task<Category> GetCategoryByNameAsync(string categoryName)
        {
            ThrowIfDisposed();
            if (categoryName == null) throw new ArgumentNullException(nameof(categoryName));

            return await _repository.FindCategoryByNameAsync(categoryName, CancellationToken);
        }

        public async Task<OperationResult> CreateCategoryAsync(Category category)
        {
            ThrowIfDisposed();
            if (category == null) throw new ArgumentNullException(nameof(category));

            var result = await Validate(new[] {category}, CategoryValidators);
            if (!result.Succeeded)
            {
                return result;
            }

            result = await _repository.CreateCategoryAsync(category, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> UpdateCategoryAsync(Category category)
        {
            ThrowIfDisposed();
            if (category == null) throw new ArgumentNullException(nameof(category));

            var result = await Validate(new[] {category}, CategoryValidators);
            if (!result.Succeeded)
            {
                return result;
            }

            result = await _repository.UpdateCategoryAsync(category, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteCategoryAsync(int id)
        {
            ThrowIfDisposed();

            var category = await _repository.CategoriesQueryable
                .Where(x => x.Id == id)
                .Include(x => x.Children)
                .FirstOrDefaultAsync(CancellationToken);

            if (category == null)
            {
                return OperationResult.Failure(ErrorDescriber.EntityNotFound("Категория"));
            }

            if (_repository.ProductCategoriesQueryable.Any(x => x.CategoryId == id))
            {
                return OperationResult.Failure(ErrorDescriber.CannotDeleteCategoryWithProducts());
            }

            using (var tran = _repository.BeginTransaction())
            {
                foreach (var child in category.Children.ToList()
                ) //to list or "Collection was modified; enumeration operation may not execute. exception"
                {
                    child.ParentId = category.ParentId;
                    var updateRes = await _repository.UpdateCategoryAsync(child, CancellationToken);
                    if (!updateRes.Succeeded)
                    {
                        return updateRes;
                    }
                }

                var result = await _repository.DeleteCategoryAsync(id, CancellationToken);
                if (!result.Succeeded)
                {
                    return result;
                }

                tran.Commit();
            }

            return OperationResult.Success();
        }

        public async Task<IEnumerable<Category>> SearchCategoriesByName(string search)
        {
            ThrowIfDisposed();
            if (search == null)
                throw new ArgumentNullException(nameof(search));

            if (search == "")
                return Enumerable.Empty<Category>();

            return await _repository.CategoriesQueryable
                .Where(x => x.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToListAsync(CancellationToken);
        }

        public async Task<IEnumerable<DescriptionGroup>> GetAllDescriptionGroupsAsync()
        {
            ThrowIfDisposed();
            return await _repository.FindDescriptionGroupsAsync(CancellationToken);
        }

        public async Task<IEnumerable<(DescriptionGroup, int)>> GetAllDescriptionGroupsWithProductCountAsync()
        {
            ThrowIfDisposed();
            var groups = await _repository.FindDescriptionGroupsAsync(CancellationToken);
            var result = new List<(DescriptionGroup, int)>();
            foreach (var group in groups)
            {
                var prodCount = await _repository.DescriptionGroupsQueryable
                    .Where(x => x.Id == group.Id)
                    .SelectMany(x => x.DescriptionGroupItems)
                    .SelectMany(x => x.Descriptions)
                    .Select(x => x.ProductId)
                    .Distinct()
                    .CountAsync(CancellationToken);
                result.Add((group, prodCount));
            }

            return result;
        }

        public async Task<OperationResult> CreateDescriptionGroupAsync(DescriptionGroup descriptionGroup)
        {
            ThrowIfDisposed();
            if (descriptionGroup == null) throw new ArgumentNullException(nameof(descriptionGroup));

            if (await _repository.DescriptionGroupItemsQueryable
                .AnyAsync(x => x.Name == descriptionGroup.Name, CancellationToken))
            {
                return OperationResult.Failure(ErrorDescriber.DuplicateDescriptionGroupName());
            }

            var result = await Validate(new[] {descriptionGroup}, DescriptionGroupValidators);
            if (!result.Succeeded)
            {
                return result;
            }

            result = await _repository.CreateDescriptionGroupAsync(descriptionGroup, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> UpdateDescriptionGroupAsync(DescriptionGroup descriptionGroup)
        {
            ThrowIfDisposed();
            if (descriptionGroup == null) throw new ArgumentNullException(nameof(descriptionGroup));

            var result = await Validate(new[] {descriptionGroup}, DescriptionGroupValidators);
            if (!result.Succeeded)
            {
                return result;
            }

            result = await _repository.UpdateDescriptionGroupAsync(descriptionGroup, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteDescriptionGroupAsync(int id)
        {
            ThrowIfDisposed();
            var descGroup = await _repository.DescriptionGroupsQueryable
                .FirstOrDefaultAsync(x => x.Id == id, CancellationToken);

            if (descGroup == null)
            {
                return OperationResult.Failure(ErrorDescriber.EntityNotFound("Группа описаний"));
            }

            var containsDescriptions = await _repository.DescriptionGroupsQueryable
                .Where(x => x.Id == id)
                .SelectMany(x => x.DescriptionGroupItems)
                .SelectMany(x => x.Descriptions)
                .AnyAsync(CancellationToken);

            if (containsDescriptions)
            {
                return OperationResult.Failure(ErrorDescriber.CannotDeleteDescGroupWithProducts());
            }

            var result = await _repository.DeleteDescriptionGroupAsync(id, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return OperationResult.Success();
        }

        public async Task<DescriptionGroup> GetDescriptionGroupByIdAsync(int id)
        {
            return await _repository.FindDescriptionGroupByIdAsync(id, CancellationToken);
        }

        public async Task<DescriptionGroup> GetDescriptionGroupByNameAsync(string name)
        {
            return await _repository.FindDescriptionGroupByNameAsync(name, CancellationToken);
        }

        public async Task<DescriptionGroupItem> GetDescriptionItemByNameAsync(string name)
        {
            return await _repository.FindDescriptionGroupItemByNameAsync(name, CancellationToken);
        }

        public async Task<OperationResult> CreateDescriptionItemAsync(DescriptionGroupItem descriptionGroupItem)
        {
            ThrowIfDisposed();
            if (descriptionGroupItem == null) throw new ArgumentNullException(nameof(descriptionGroupItem));

            var result = await Validate(new[] {descriptionGroupItem}, DescriptionGroupItemValidators);
            if (!result.Succeeded)
            {
                return result;
            }

            result = await _repository.CreateDescriptionGroupItemAsync(descriptionGroupItem, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> UpdateDescriptionItemAsync(DescriptionGroupItem descriptionGroupItem)
        {
            ThrowIfDisposed();
            if (descriptionGroupItem == null) throw new ArgumentNullException(nameof(descriptionGroupItem));

            var result = await Validate(new[] {descriptionGroupItem}, DescriptionGroupItemValidators);
            if (!result.Succeeded)
            {
                return result;
            }

            result = await _repository.UpdateDescriptionGroupItemAsync(descriptionGroupItem, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteDescriptionItemAsync(int id)
        {
            ThrowIfDisposed();
            var descGroupItem = await _repository.DescriptionGroupItemsQueryable
                .FirstOrDefaultAsync(x => x.Id == id, CancellationToken);

            if (descGroupItem == null)
            {
                return OperationResult.Failure(ErrorDescriber.EntityNotFound("Группа описаний"));
            }

            var containsDescriptions = await _repository.DescriptionGroupItemsQueryable
                .Where(x => x.Id == id)
                .SelectMany(x => x.Descriptions)
                .AnyAsync(CancellationToken);

            if (containsDescriptions)
            {
                return OperationResult.Failure(ErrorDescriber.CannotDeleteDescItemsWithProducts());
            }

            var result = await _repository.DeleteDescriptionGroupItemAsync(id, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return OperationResult.Success();
        }

        public async Task<DescriptionGroupItem> GetDescriptionItemByIdAsync(int id)
        {
            ThrowIfDisposed();
            return await _repository.FindDescriptionGroupItemByIdAsync(id, CancellationToken);
        }

        public async Task<IEnumerable<DescriptionGroupItem>> GetDescriptionItemsAsync(int groupId)
        {
            ThrowIfDisposed();
            return await _repository.FindDescriptionGroupItemsAsync(groupId, CancellationToken);
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

        public async Task<IEnumerable<Product>> GetNewProducts(int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
            ThrowIfDisposed();


            var newProducts = await _repository.ProductsQueryable
                .Where(x=>x.Available)
                .OrderByDescending(x => x.Created)
                .Take(count)
                .Include(x => x.Images)
                .ToListAsync(CancellationToken);
            return newProducts;
        }

        public void TransformImages(IEnumerable<Image> images)
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

        public async Task<OperationResult> Validate<T>(
            IEnumerable<T> items, IEnumerable<IShopValidator<T>> validators,
            Func<T, bool> predicateBeforeValidation = null) where T : class
        {
            var errors = new List<OperationError>();
            var itemArray = items as T[] ?? items.ToArray();
            foreach (var validator in validators)
            {
                foreach (var item in itemArray)
                {
                    if (!(predicateBeforeValidation?.Invoke(item) ?? true)) continue;
                    var result = await validator.ValidateAsync(this, item);
                    if (!result.Succeeded)
                    {
                        errors.AddRange(result.Errors);
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