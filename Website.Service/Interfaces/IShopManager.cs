﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Website.Service.DTO;
using Website.Service.Enums;
using Website.Service.Infrastructure;

namespace Website.Service.Interfaces
{
    public interface IShopManager
    {
        /// <summary>
        /// Creates product with images and category in db.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OperationResult> CreateItemAsync(ProductDTO product);

        Task<SortPageResult<ProductDTO>> GetSortFilterPageAsync(ItemTypeSelector types, string search, string sortOrder, int currPage, int countPerPage);

        Task<ProductDTO> GetProductById(int id);

        Task<List<DescriptionGroupDTO>> GetProductDescriptions(int productId);
    }
}