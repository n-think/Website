using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Website.Core.Infrastructure;

namespace Website.Core.Interfaces.Repositories
{
    public interface IDescriptionsRepository<TDescriptionGroup, TDescriptionGroupItem, TDescription>
    {
         #region Descriptions

        IQueryable<TDescription> DescriptionsQueryable { get; }

        Task<List<TDescription>> GetProductDescriptions(int productId,
            CancellationToken cancellationToken);

        Task<OperationResult> CreateProductDescriptions(int productId,
            IEnumerable<TDescription> newDescriptions, CancellationToken cancellationToken);

        Task<OperationResult> UpdateProductDescriptions(int productId,
            IEnumerable<TDescription> descriptionsToUpdate, CancellationToken cancellationToken);

        Task<OperationResult> DeleteProductDescriptions(int productId,
            IEnumerable<TDescription> newDescriptions, CancellationToken cancellationToken);

        #endregion

        #region DescriptionGroups

        Task<IEnumerable<TDescriptionGroup>> FindDescriptionGroupsAsync(CancellationToken cancellationToken);

        Task<IEnumerable<TDescriptionGroupItem>> FindDescriptionGroupItemsAsync(int groupId,
            CancellationToken cancellationToken);

        IQueryable<TDescriptionGroup> DescriptionGroupsQueryable { get; }
        IQueryable<TDescriptionGroupItem> DescriptionGroupItemsQueryable { get; }

        Task<TDescriptionGroup> FindDescriptionGroupByIdAsync(int id, CancellationToken cancellationToken);
        Task<TDescriptionGroup> FindDescriptionGroupByNameAsync(string name, CancellationToken cancellationToken);

        Task<OperationResult> CreateDescriptionGroupAsync(TDescriptionGroup descriptionGroup,
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateDescriptionGroupAsync(TDescriptionGroup descriptionGroup,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteDescriptionGroupAsync(int id, CancellationToken cancellationToken);

        Task<TDescriptionGroupItem> FindDescriptionGroupItemByIdAsync(int id, CancellationToken cancellationToken);

        Task<TDescriptionGroupItem>
            FindDescriptionGroupItemByNameAsync(string name, CancellationToken cancellationToken);

        Task<OperationResult> CreateDescriptionGroupItemAsync(TDescriptionGroupItem descriptionGroupItem,
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateDescriptionGroupItemAsync(TDescriptionGroupItem descriptionGroupItem,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteDescriptionGroupItemAsync(int id, CancellationToken cancellationToken);

        #endregion
    }
}