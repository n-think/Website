using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Website.Data.EF.Models;
using Website.Service.DTO;
using Website.Service.Enums;
using Website.Service.Infrastructure;

namespace Website.Service.Interfaces
{
    public interface IShopStore<TDtoProduct, TDtoImage, TDtoCategory, TDtoOrder> : IDisposable
        where TDtoProduct : class
        where TDtoImage : class
        where TDtoCategory : class
        where TDtoOrder : class
    {
        #region CRUD product
        Task<OperationResult> CreateProductAsync(TDtoProduct product, CancellationToken cancellationToken);
        Task<TDtoProduct> FindProductByIdAsync(int productId, CancellationToken cancellationToken);
        Task<TDtoProduct> FindProductByNameAsync(string productName, CancellationToken cancellationToken);
        Task<OperationResult> UpdateProductAsync(TDtoProduct product, CancellationToken cancellationToken);
        Task<OperationResult> DeleteProductAsync(TDtoProduct product, CancellationToken cancellationToken);
        #endregion

        #region CRUD category
        Task<OperationResult> CreateCategoryAsync(TDtoCategory category, CancellationToken cancellationToken);
        Task<TDtoCategory> FindCategoryByNameAsync(string categoryName, CancellationToken cancellationToken);
        Task<OperationResult> UpdateCategoryAsync(TDtoCategory category, CancellationToken cancellationToken);
        Task<OperationResult> DeleteCategoryAsync(TDtoCategory category, CancellationToken cancellationToken);
        #endregion

        #region images
        Task<OperationResult> SaveImagesAsync(ProductDto product, CancellationToken cancellationToken);
        Task<OperationResult> LoadImagesAsync(ProductDto product, CancellationToken cancellationToken);
        #endregion

        #region Descriptions
        Task<OperationResult> LoadProductDescriptions(TDtoProduct product, CancellationToken cancellationToken);
        #endregion

        Task<SortPageResult<TDtoProduct>> SortFilterPageResultAsync(ItemTypeSelector types, string searchString, string sortPropName,
            int currentPage, int countPerPage, CancellationToken cancellationToken);
    }
}