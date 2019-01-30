using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Website.Core.Infrastructure;

namespace Website.Core.Interfaces.Repositories
{
    public interface IImageRepository<TImage,TImageData>
    {
        IQueryable<TImage> ImagesQueryable { get; }
        IQueryable<TImageData> ImageDataQueryable { get; }

        Task<OperationResult> CreateImagesAsync(int productId, IEnumerable<TImage> images,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteImagesAsync(int productId, IEnumerable<TImage> images,
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateImagesAsync(int productId, IEnumerable<TImage> newImages,
            CancellationToken cancellationToken);
    }
}