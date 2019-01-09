using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Website.Core.Infrastructure;
using Website.Core.Models.Domain;

namespace Website.Core.Interfaces.Repositories
{
    public interface
        IShopRepository<TProduct, TProductImage, TCategory, TDescriptionGroup, TDescription, TOrder> : IDisposable
        where TProduct : class
        where TProductImage : class
        where TCategory : class
        where TDescriptionGroup : class
        where TDescription : class
        where TOrder : class
    {

        IDbContextTransaction BeginTransaction(IsolationLevel iLevel = IsolationLevel.Serializable);
        void JoinTransaction(IDbContextTransaction tran);
        
        //TODO split repos

        #region product

        IQueryable<TProduct> ProductsQueryable { get; }
        Task<OperationResult> CreateProductAsync(TProduct product, CancellationToken cancellationToken);

        Task<TProduct> FindProductByIdAsync(int productId, bool loadImages, bool loadDescriptions,
            bool loadCategories, CancellationToken cancellationToken);

        Task<TProduct> FindProductByNameAsync(string productName, bool loadImages, bool loadDescriptions,
            bool loadCategories, CancellationToken cancellationToken);

        Task<Product> FindProductByCodeAsync(int productCode, bool loadImages,
            bool loadDescriptions, bool loadCategories,
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateProductAsync(TProduct product, CancellationToken cancellationToken);
        Task<OperationResult> DeleteProductAsync(TProduct product, CancellationToken cancellationToken);

        #endregion

        #region categories

        IQueryable<TCategory> CategoriesQueryable { get; }
        Task<OperationResult> CreateCategoryAsync(TCategory category, CancellationToken cancellationToken);
        Task<TCategory> FindCategoryByNameAsync(string categoryName, CancellationToken cancellationToken);
        Task<OperationResult> UpdateCategoryAsync(TCategory category, CancellationToken cancellationToken);
        Task<OperationResult> DeleteCategoryAsync(TCategory category, CancellationToken cancellationToken);
        Task<IEnumerable<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken);

        Task<OperationResult> AddProductCategoriesAsync(int productId, IEnumerable<ProductToCategory> categories,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteProductCategoriesAsync(int productId, IEnumerable<ProductToCategory> categories,
            CancellationToken cancellationToken);

        #endregion

        #region images

        IQueryable<TProductImage> ImagesQueryable { get; }

        Task<OperationResult> CreateImagesAsync(int productId, IEnumerable<TProductImage> images,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteImagesAsync(int productId, IEnumerable<TProductImage> images,
            CancellationToken cancellationToken);

        #endregion

        #region Descriptions

        IQueryable<TDescription> DescriptionsQueryable { get; }
        IQueryable<TDescriptionGroup> DescriptionGroupsQueryable { get; }

        Task<OperationResult> CreateProductDescriptions(int productId,
            IEnumerable<Description> newDescriptions, CancellationToken cancellationToken);

        Task<OperationResult> DeleteProductDescriptions(int productId,
            IEnumerable<Description> newDescriptions, CancellationToken cancellationToken);

        Task<List<TDescription>> GetProductDescriptions(int productId, CancellationToken cancellationToken);
        Task<IEnumerable<TDescriptionGroup>> GetAllDescriptionGroupsAsync(CancellationToken cancellationToken);
        #endregion
    }
}