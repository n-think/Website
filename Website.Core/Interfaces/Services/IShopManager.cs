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
        OperationErrorDescriber ErrorDescriber { get; }

        Task<OperationResult> CreateProductAsync(Product product, IEnumerable<Image> images,
            IEnumerable<int> categoryIdsToAdd, IEnumerable<Description> descriptions);
        Task<Product> GetProductByIdAsync(int id, bool loadImages = false, bool loadDescriptions = false, bool loadCategories = false);
        Task<Product> GetProductByNameAsync(string name, bool loadImages, bool loadDescriptions, bool loadCategories);
        Task<Product> GetProductByCodeAsync(int code, bool loadImages, bool loadDescriptions, bool loadCategories);
        Task<OperationResult> UpdateProductAsync(Product product,
            IEnumerable<Description> descriptionsToUpdate,
            IEnumerable<int> categoryIdsToUpdate,
            IEnumerable<Image> imagesToUpdate);
        Task<OperationResult> DeleteProductAsync(int productId);

        Task<SortPageResult<Product>> GetSortFilterPageAsync(ItemTypeSelector types, string search, string sortOrder,
            int currPage, int countPerPage, int[] categoryIds, int[] descGroupIds);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<IEnumerable<(Category, int)>> GetAllCategoriesWithProductCountAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> GetCategoryByNameAsync(string categoryName);
        Task<OperationResult> CreateCategoryAsync(Category category);
        Task<OperationResult> UpdateCategoryAsync(Category category);
        Task<OperationResult> DeleteCategoryAsync(int id);
        
        Task<IEnumerable<DescriptionGroup>> GetAllDescriptionGroupsAsync();
        Task<IEnumerable<(DescriptionGroup, int)>> GetAllDescriptionGroupsWithProductCountAsync();
        Task<IEnumerable<DescriptionGroupItem>> GetDescriptionItemsAsync(int groupId);
        
       Task<(byte[], string)> GetImageDataMimeAsync(int imageId, bool thumb = false);
    }
}