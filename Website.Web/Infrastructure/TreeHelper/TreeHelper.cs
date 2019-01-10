using System;
using System.Collections.Generic;
using System.Linq;
using Website.Web.Models.DTO;

namespace Website.Web.Infrastructure.TreeHelper
{
    public static class TreeHelper
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
        
//        public static List<DescriptionGroupDto> ToTree(this IEnumerable<DescriptionGroupDto> descGroups)
//        {
//            if (descGroups == null)
//                throw new ArgumentNullException(nameof(descGroups));
//
//            var items = descGroups
//                .ToDictionary(x => x.Id);
//            
//            var tree = new List<DescriptionGroupDto>();
//            foreach (var descGroup in items.Values)
//            {
//                if (descGroup.ParentId.HasValue)
//                {
//                    var parentObj = items[descGroup.ParentId.Value];
//                    parentObj.Children.Add(descGroup);
//                }
//                else
//                {
//                    tree.Add(descGroup);
//                }
//            }
//            return tree;
//        }
        
        public static IEnumerable<T> GetNodeAndChildren<T>(T node) where T : ITreeItem<T>
        {
            yield return node;
            foreach (var child in node.Children)
            {
                foreach (var x in GetNodeAndChildren<T>(child))
                {
                    yield return x;
                }
            }
        }
    }
}
