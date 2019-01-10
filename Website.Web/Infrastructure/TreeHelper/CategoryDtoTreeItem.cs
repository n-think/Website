using System.Collections.Generic;
using Website.Web.Models.DTO;

namespace Website.Web.Infrastructure.TreeHelper
{
    public class CategoryDtoTreeItem : CategoryDto, ITreeItem<CategoryDtoTreeItem>
    {
        public List<CategoryDtoTreeItem> Children { get; set; }
    }
}
