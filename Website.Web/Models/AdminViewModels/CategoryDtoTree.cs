using System;
using System.Collections.Generic;
using System.Linq;
using Website.Web.Models.DTO;

namespace Website.Web.Models.AdminViewModels
{
    internal static class CategoryDtoTree
    {
        public static List<CategoryDtoTreeItem> ToTree(this IEnumerable<CategoryDto> categories)
        {
            if (categories == null)
                throw new ArgumentNullException(nameof(categories));

            var items = categories
                .Select(x=> new CategoryDtoTreeItem
                {
                    Id = x.Id,
                    Description = x.Description,
                    Name = x.Name,
                    ParentId = x.ParentId,
                    DtoState = x.DtoState,
                    ProductCount = x.ProductCount,
                    Timestamp = x.Timestamp,
                    Children = new List<CategoryDtoTreeItem>()
                })
                .ToDictionary(x => x.Id);
            var tree = new List<CategoryDtoTreeItem>();
            foreach (var cat in items.Values)
            {
                // if the element has a parent id
                if (cat.ParentId.HasValue)
                {
                    // find the parent object …
                    var parentObj = items[cat.ParentId.Value];
                    // … and add this object to the parent’s child list
                    parentObj.Children.Add(cat);
                }
                else
                {
                    // otherwise this is a root element, so add it to the target list
                    tree.Add(cat);
                }
            }
            return tree;
        }
    }
}
