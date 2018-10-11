using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Website.Data.EF.Models;
using Website.Service.DTO;
using Website.Service.Infrastructure;

namespace Website.Service.Interfaces
{
    public interface IShopStore<TDtoProduct> : IDisposable where TDtoProduct : class
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

        Task<OperationResult> CreateProductAsync(TDtoProduct product, CancellationToken cancellationToken = default(CancellationToken));
        Task<TDtoProduct> FindProductByIdAsync(string productId, CancellationToken cancellationToken = default(CancellationToken));
        Task<OperationResult> UpdateProductAsync(TDtoProduct product, CancellationToken cancellationToken = default(CancellationToken));
        Task<OperationResult> RemoveProductAsync(string productId, CancellationToken cancellationToken = default(CancellationToken));
        Task<OperationResult> CreateCategoryAsync(string categoryName, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> FindCategoryByNameAsync(string categoryName, CancellationToken cancellationToken = default(CancellationToken));
        Task<OperationResult> RemoveCategoryAsync(string categoryName, CancellationToken cancellationToken = default(CancellationToken));
        Task<OperationResult> SaveImage(Bitmap image, string savePath, CancellationToken cancellationToken = default(CancellationToken));
        Task<OperationResult> RemoveImage(Bitmap image, string removePath, CancellationToken cancellationToken = default(CancellationToken));
    }
}