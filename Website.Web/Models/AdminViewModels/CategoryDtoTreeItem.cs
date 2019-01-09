using System.Collections.Generic;
using Website.Web.Models.DTO;

namespace Website.Web.Models.AdminViewModels
{
    public class CategoryDtoTreeItem : CategoryDto
    {
        public List<CategoryDtoTreeItem> Children { get; set; }
    }
}
