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
    public interface IShopStore<TDtoProduct, TDbProduct> : IDisposable
        where TDtoProduct : class
        where TDbProduct : class
    {
        /// <summary>
        /// Gets or sets a flag indicating if changes should be persisted after CreateAsync, UpdateAsync and DeleteAsync are called.
        /// </summary>
        /// <value>
        /// True if changes should be automatically persisted, otherwise false.
        /// </value>
        bool AutoSaveChanges { get; set; }

        IQueryable<TDbProduct> ProductsQueryable { get; }

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
        void FilterProducstTypeQuery(ItemTypeSelector types, ref IQueryable<Product> prodQuery);
        void SearchProductsQuery(string searchString, ref IQueryable<Product> prodQuery);
        void OrderProductsQuery(string sortPropName, ref IQueryable<Product> prodQuery);
        Task<int> CountQueryAsync(IQueryable<Product> prodQuery, CancellationToken cancellationToken = default(CancellationToken));
        void SkipTakeQuery(int skip, int take, ref IQueryable<Product> prodQuery);
        Task<IEnumerable<ProductDTO>> ExecuteProductsQuery(IQueryable<Product> prodQuery, CancellationToken cancellationToken = default(CancellationToken));
    }
}