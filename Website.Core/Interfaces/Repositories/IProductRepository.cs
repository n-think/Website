using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Website.Core.Infrastructure;
using Website.Core.Models.Domain;

namespace Website.Core.Interfaces.Repositories
{
    public interface IProductRepository<TProduct>
    {
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
    }
}