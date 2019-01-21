﻿using System.Collections.Generic;
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
        Task<Product> GetProductByIdAsync(int id, bool loadImages, bool loadDescriptions, bool loadCategories);
        Task<Product> GetProductByNameAsync(string name, bool loadImages, bool loadDescriptions, bool loadCategories);
        Task<Product> GetProductByCodeAsync(int code, bool loadImages, bool loadDescriptions, bool loadCategories);
        Task<OperationResult> UpdateProductAsync(Product product,
            IEnumerable<Description> descriptionsToUpdate,
            IEnumerable<int> categoryIdsToUpdate,
            IEnumerable<Image> imagesToUpdate);
        Task<OperationResult> DeleteProductAsync(int productId);
        Task<IEnumerable<Product>> GetNewProducts(int count);
        Task<IEnumerable<Product>> SearchProductsByName(string searchString);

        Task<SortPageResult<Product>> GetSortFilterPageAsync(ItemTypeSelector types, int currPage, int countPerPage,
            string search = null, string sortOrder = null, int[] categoryIds = null, int[] descGroupIds = null);
        
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<IEnumerable<(Category, int)>> GetAllCategoriesWithProductCountAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> GetCategoryByNameAsync(string categoryName);
        Task<OperationResult> CreateCategoryAsync(Category category);
        Task<OperationResult> UpdateCategoryAsync(Category category);
        Task<OperationResult> DeleteCategoryAsync(int id);
        Task<IEnumerable<Category>> SearchCategoriesByName(string search);
       
       Task<OperationResult> CreateDescriptionGroupAsync(DescriptionGroup descriptionGroup);
       Task<OperationResult> UpdateDescriptionGroupAsync(DescriptionGroup descriptionGroup);
       Task<OperationResult> DeleteDescriptionGroupAsync(int id);
       Task<DescriptionGroup> GetDescriptionGroupByIdAsync(int id);
       Task<DescriptionGroup> GetDescriptionGroupByNameAsync(string name);
       Task<IEnumerable<DescriptionGroup>> GetAllDescriptionGroupsAsync();
       Task<IEnumerable<(DescriptionGroup, int)>> GetAllDescriptionGroupsWithProductCountAsync();
       
       Task<OperationResult> CreateDescriptionItemAsync(DescriptionGroupItem descriptionGroupItem);
       Task<OperationResult> UpdateDescriptionItemAsync(DescriptionGroupItem descriptionGroupItem);
       Task<OperationResult> DeleteDescriptionItemAsync(int id);
       Task<DescriptionGroupItem> GetDescriptionItemByIdAsync(int id);
       Task<DescriptionGroupItem> GetDescriptionItemByNameAsync(string name);
       Task<IEnumerable<DescriptionGroupItem>> GetDescriptionItemsAsync(int groupId);
       
       Task<(byte[], string)> GetImageDataMimeAsync(int imageId, bool thumb = false);
    }
}