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
    public interface IShopStore<TDtoProduct> : IDisposable
        where TDtoProduct : class
    {
        /// <summary>
        /// Gets or sets a flag indicating if changes should be persisted after CreateAsync, UpdateAsync and DeleteAsync are called.
        /// </summary>
        /// <value>
        /// True if changes should be automatically persisted, otherwise false.
        /// </value>
        bool AutoSaveChanges { get; set; }

        /// <summary>Saves the current store.</summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task SaveChanges(CancellationToken cancellationToken);

        Task<OperationResult> CreateProductAsync(TDtoProduct product, CancellationToken cancellationToken);
        Task<TDtoProduct> FindProductByIdAsync(int productId, CancellationToken cancellationToken);
        Task<OperationResult> UpdateProductAsync(TDtoProduct product, CancellationToken cancellationToken);
        Task<OperationResult> RemoveProductAsync(int productId, CancellationToken cancellationToken);
        Task<OperationResult> CreateCategoryAsync(string categoryName, CancellationToken cancellationToken);
        Task<bool> FindCategoryByNameAsync(string categoryName, CancellationToken cancellationToken);
        Task<OperationResult> RemoveCategoryAsync(string categoryName, CancellationToken cancellationToken);
        Task<OperationResult> SaveImage(Bitmap image, string savePath, CancellationToken cancellationToken);
        Task<OperationResult> RemoveImage(Bitmap image, string removePath, CancellationToken cancellationToken);
        Task<SortPageResult<ProductDTO>> SortFilterPageResultAsync(ItemTypeSelector types, string searchString, string sortPropName,
            int currentPage, int countPerPage, CancellationToken cancellationToken);
    }
}