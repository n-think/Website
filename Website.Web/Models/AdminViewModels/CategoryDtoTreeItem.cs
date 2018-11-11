using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Service.DTO;

namespace Website.Web.Models.AdminViewModels
{
    public class CategoryDtoTreeItem : CategoryDto
    {
        public List<CategoryDtoTreeItem> Children { get; set; }
    }
}
