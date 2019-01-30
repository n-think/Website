using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Website.Core.Infrastructure;
using Website.Core.Models.Domain;

namespace Website.Core.Interfaces.Repositories
{
    public interface ICategoryRepository<TCategory, out TProductCategory>
    {
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
    }
}