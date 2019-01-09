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
        public ShopManager(IShopRepository<Product, Image, Category, DescriptionGroup,
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
            IShopImageTransformer<ImageBinData> imgTransformer,
            OperationErrorDescriber errorDescriber = null)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _errorDescriber = errorDescriber ?? new OperationErrorDescriber();
            Options = optionsAccessor.Value ?? new ShopManagerOptions();
            CancellationToken = context?.HttpContext?.RequestAborted ?? CancellationToken.None;
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

        private readonly IShopRepository<Product, Image, Category, DescriptionGroup,
            Description, Order> _repository;

        private readonly OperationErrorDescriber _errorDescriber;
        private readonly ILogger<ShopManager> _logger;
        public ShopManagerOptions Options { get; }
        public IList<IShopValidator<Product>> ProductValidators { get; } = new List<IShopValidator<Product>>();
        public IList<IShopValidator<Image>> ImageValidators { get; } = new List<IShopValidator<Image>>();
        public IList<IShopValidator<Category>> CategoryValidators { get; } = new List<IShopValidator<Category>>();

        public IList<IShopValidator<DescriptionGroup>> DescriptionGroupValidators { get; } =
            new List<IShopValidator<DescriptionGroup>>();

        public IList<IShopValidator<Description>> DescriptionValidators { get; } =
            new List<IShopValidator<Description>>();

        public IList<IShopValidator<Order>> OrderValidators { get; } = new List<IShopValidator<Order>>();
        public IShopImageTransformer<ImageBinData> ImageTransformer { get; }
        protected CancellationToken CancellationToken { get; }

        public async Task<OperationResult> CreateProductAsync(Product product)
        {
            ThrowIfDisposed();
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var result = await ValidateProduct(product);
            if (!result.Succeeded)
            {
                return result;
            }

            result = await ValidateProductImages(product.Images);
            if (!result.Succeeded)
            {
                return result;
            }

            TransformImages(product.Images);

            using (var tran = _repository.BeginTransaction())
            {
                result = await _repository.CreateProductAsync(product, CancellationToken);
                if (!result.Succeeded)
                {
                    return result;
                }

                result =
                    await _repository.CreateImagesAsync(product.Id, product.Images, CancellationToken);
                if (!result.Succeeded)
                {
                    return result;
                }

                tran.Commit();
            }

            return OperationResult.Success();
        }

        public async Task<OperationResult> UpdateProductAsync(Product product, IEnumerable<Image> imagesToAdd,
            IEnumerable<Image> imagesToRemove)
        {
            ThrowIfDisposed();
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            if (product.Id == 0)
            {
                return OperationResult.Failure(_errorDescriber.InvalidProductId());
            }

            var result = await ValidateProduct(product);

            if (!result.Succeeded)
            {
                return result;
            }

            var imgArrayToAdd = imagesToAdd as Image[] ?? imagesToAdd.ToArray();
            result = await ValidateProductImages(imgArrayToAdd);
            if (!result.Succeeded)
            {
                return result;
            }

            TransformImages(imgArrayToAdd);

            using (var tran = _repository.BeginTransaction())
            {
                result = await _repository.UpdateProductAsync(product, CancellationToken);
                if (!result.Succeeded)
                {
                    return result;
                }

                result = await _repository.CreateImagesAsync(product.Id, imgArrayToAdd, CancellationToken);
                if (!result.Succeeded)
                {
                    return result;
                }

                result = await _repository.DeleteImagesAsync(product.Id, imagesToRemove, CancellationToken);
                if (!result.Succeeded)
                {
                    return result;
                }

                tran.Commit();
            }


            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteProductAsync(Product product)
        {
            ThrowIfDisposed();
            if (product?.Id == null)
                throw new ArgumentException(nameof(product));

            if (product.Available)
            {
                return OperationResult.Failure(_errorDescriber.CannotDeleteActiveProduct());
            }

            var result = await _repository.DeleteProductAsync(product, CancellationToken);
            if (!result.Succeeded)
            {
                return OperationResult.Failure(_errorDescriber.ErrorDeletingProduct());
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
            return await _repository.GetAllDescriptionGroupsAsync(CancellationToken);
        }

        public async Task<IEnumerable<DescriptionGroup>> GetDescGroupFirstChildren(int groupId)
        {
            ThrowIfDisposed();
            if (groupId < 0)
                throw new ArgumentException(nameof(groupId));
            
            return await _repository.DescriptionGroupsQueryable
                .Where(x => x.ParentId == groupId)
                .ToListAsync(CancellationToken);
        }

        private void TransformImages(IEnumerable<Image> images)
        {
            if (images == null)
                return;

            foreach (var image in images)
            {
                if (image.BinData == null)
                {
                    throw new ArgumentException(nameof(image.BinData));
                }

                ImageTransformer.ProcessImage(image.BinData);
            }
        }

        private async Task<OperationResult> ValidateProduct(Product product)
        {
            var errors = new List<OperationError>();
            foreach (var validator in ProductValidators)
            {
                var result = await validator.ValidateAsync(this, product);
                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors);
                }
            }

            if (errors.Count > 0)
            {
                return OperationResult.Failure(errors.ToArray());
            }

            return OperationResult.Success();
        }

        private async Task<OperationResult> ValidateProductImages(IEnumerable<Image> images)
        {
            var errors = new List<OperationError>();
            var imgArray = images as Image[] ?? images.ToArray();
            foreach (var validator in ImageValidators)
            {
                foreach (var image in imgArray)
                {
                    var result = await validator.ValidateAsync(this, image);
                }
            }

            if (errors.Count > 0)
            {
                return OperationResult.Failure(errors.ToArray());
            }

            return OperationResult.Success();
        }

        private async Task<OperationResult> ValidateCategories(IEnumerable<Category> categories)
        {
            throw new NotImplementedException();
        }

        private async Task<OperationResult> ValidateDescriptionGroups(IEnumerable<DescriptionGroup> descriptionGroups)
        {
            throw new NotImplementedException();
        }

        private async Task<OperationResult> ValidateDescriptions(IEnumerable<Description> descriptions)
        {
            throw new NotImplementedException();
        }

        private async Task<OperationResult> ValidateOrder(Order order)
        {
            throw new NotImplementedException();
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