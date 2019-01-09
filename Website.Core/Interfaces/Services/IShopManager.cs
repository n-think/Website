using System.Collections.Generic;
using System.Threading.Tasks;
using Website.Core.Enums;
using Website.Core.Infrastructure;
using Website.Core.Models.Domain;

namespace Website.Core.Interfaces.Services
{
    public interface IShopManager
    {
        //TODO interface
        ShopManagerOptions Options { get; }

        Task<OperationResult> CreateProductAsync(Product product);
        Task<Product> GetProductByIdAsync(int id, bool loadImages = false, bool loadDescriptions = false, bool loadCategories = false);
        Task<Product> GetProductByNameAsync(string name, bool loadImages, bool loadDescriptions, bool loadCategories);
        Task<Product> GetProductByCodeAsync(int code, bool loadImages, bool loadDescriptions, bool loadCategories);
        Task<OperationResult> UpdateProductAsync(Product product, IEnumerable<Image> imagesToAdd,
            IEnumerable<Image> imagesToRemove);
        Task<OperationResult> DeleteProductAsync(Product product);

        Task<SortPageResult<Product>> GetSortFilterPageAsync(ItemTypeSelector types, string search, string sortOrder, int currPage, int countPerPage);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<IEnumerable<(Category, int)>> GetAllCategoriesWithProductCountAsync();
        Task<IEnumerable<DescriptionGroup>> GetAllDescriptionGroupsAsync();
        Task<IEnumerable<DescriptionGroup>> GetDescGroupFirstChildren(int groupId);
    }
}