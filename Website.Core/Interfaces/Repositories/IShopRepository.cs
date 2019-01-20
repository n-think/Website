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
        IShopRepository<TProduct, TImage, TImageData, TCategory, TProductCategory, TDescriptionGroup, TDescriptionGroupItem, TDescription, TOrder> : IDisposable
        where TProduct : class
        where TImage : class
        where TImageData : class
        where TCategory : class
        where TProductCategory : class
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
        Task<OperationResult> DeleteProductAsync(int productId, CancellationToken cancellationToken);

        #endregion

        #region categories

        IQueryable<TCategory> CategoriesQueryable { get; }
        IQueryable<TProductCategory> ProductCategoriesQueryable { get; }
        Task<OperationResult> CreateCategoryAsync(TCategory category, CancellationToken cancellationToken);
        Task<TCategory> FindCategoryByIdAsync(int categoryId, CancellationToken cancellationToken);
        Task<TCategory> FindCategoryByNameAsync(string categoryName, CancellationToken cancellationToken);
        Task<OperationResult> UpdateCategoryAsync(TCategory category, CancellationToken cancellationToken);
        Task<OperationResult> DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken);
        Task<IEnumerable<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken);

        Task<OperationResult> AddProductCategoriesAsync(int productId, IEnumerable<int> categoryIds,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteProductCategoriesAsync(int productId, IEnumerable<int> categoryIds,
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateProductCategoriesAsync(int productId,
            IEnumerable<int> categoryIds,
            CancellationToken cancellationToken);

        #endregion

        #region images

        IQueryable<TImage> ImagesQueryable { get; }
        IQueryable<TImageData> ImageDataQueryable { get; }

        Task<OperationResult> CreateImagesAsync(int productId, IEnumerable<TImage> images,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteImagesAsync(int productId, IEnumerable<TImage> images,
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateImagesAsync(int productId, IEnumerable<Image> newImages,
            CancellationToken cancellationToken);

        #endregion

        #region Descriptions

        IQueryable<TDescription> DescriptionsQueryable { get; }

        Task<List<Description>> GetProductDescriptions(int productId,
            CancellationToken cancellationToken);
        Task<OperationResult> CreateProductDescriptions(int productId,
            IEnumerable<Description> newDescriptions, CancellationToken cancellationToken);

        Task<OperationResult> UpdateProductDescriptions(int productId,
            IEnumerable<Description> descriptionsToUpdate, CancellationToken cancellationToken);
        
        Task<OperationResult> DeleteProductDescriptions(int productId,
            IEnumerable<Description> newDescriptions, CancellationToken cancellationToken);

        #endregion

        #region DescriptionGroups

        Task<IEnumerable<DescriptionGroup>> FindDescriptionGroupsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<DescriptionGroupItem>> FindDescriptionGroupItemsAsync(int groupId, CancellationToken cancellationToken);
        
        IQueryable<TDescriptionGroup> DescriptionGroupsQueryable { get; }
        IQueryable<TDescriptionGroupItem> DescriptionGroupItemsQueryable { get; }
        
        Task<DescriptionGroup> FindDescriptionGroupByIdAsync(int id, CancellationToken cancellationToken);
        Task<DescriptionGroup> FindDescriptionGroupByNameAsync(string name, CancellationToken cancellationToken);
        Task<OperationResult> CreateDescriptionGroupAsync(DescriptionGroup descriptionGroup, CancellationToken cancellationToken);
        Task<OperationResult> UpdateDescriptionGroupAsync(DescriptionGroup descriptionGroup, CancellationToken cancellationToken);
        Task<OperationResult> DeleteDescriptionGroupAsync(int id, CancellationToken cancellationToken);
        
        Task<DescriptionGroupItem> FindDescriptionGroupItemByIdAsync(int id, CancellationToken cancellationToken);
        Task<DescriptionGroupItem> FindDescriptionGroupItemByNameAsync(string name, CancellationToken cancellationToken);
        Task<OperationResult> CreateDescriptionGroupItemAsync(DescriptionGroupItem descriptionGroupItem, CancellationToken cancellationToken);
        Task<OperationResult> UpdateDescriptionGroupItemAsync(DescriptionGroupItem descriptionGroupItem, CancellationToken cancellationToken);
        Task<OperationResult> DeleteDescriptionGroupItemAsync(int id, CancellationToken cancellationToken);
        
        #endregion


    }
}