using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Website.Service.DTO;
using Website.Service.Enums;
using Website.Service.Infrastructure;

namespace Website.Service.Interfaces
{
    public interface IShopStore<TDtoProduct, TDtoImage, TDtoCategory, TDtoDescriptionGroup, TDtoOrder> : IDisposable
        where TDtoProduct : class
        where TDtoImage : class
        where TDtoCategory : class
        where TDtoDescriptionGroup : class
        where TDtoOrder : class
    {
        #region product
        Task<OperationResult> CreateProductAsync(TDtoProduct product, CancellationToken cancellationToken);
        Task<TDtoProduct> FindProductByIdAsync(int productId, bool loadImages, bool loadDescriptions,
            bool loadCategories, CancellationToken cancellationToken);
        Task<TDtoProduct> FindProductByNameAsync(string productName, bool loadImages, bool loadDescriptions,
            bool loadCategories, CancellationToken cancellationToken);
        Task<OperationResult> UpdateProductAsync(TDtoProduct product, CancellationToken cancellationToken);
        Task<OperationResult> DeleteProductAsync(TDtoProduct product, CancellationToken cancellationToken);
        #endregion

        #region categories
        Task<OperationResult> CreateCategoryAsync(TDtoCategory category, CancellationToken cancellationToken);
        Task<TDtoCategory> FindCategoryByNameAsync(string categoryName, CancellationToken cancellationToken);
        Task<OperationResult> UpdateCategoryAsync(TDtoCategory category, CancellationToken cancellationToken);
        Task<OperationResult> DeleteCategoryAsync(TDtoCategory category, CancellationToken cancellationToken);
        Task<IEnumerable<CategoryDto>> GetAllCategories(bool getProductCount, CancellationToken cancellationToken);
        #endregion

        #region Descriptions
        Task<IEnumerable<DescriptionGroupDto>> GetDescriptionGroupsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<DescriptionItemDto>> GetDescriptionItemsAsync(int id, CancellationToken cancellationToken);
        #endregion

        Task<SortPageResult<TDtoProduct>> SortFilterPageResultAsync(ItemTypeSelector types, string searchString, string sortPropName,
            int currentPage, int countPerPage, CancellationToken cancellationToken);


    }
}