using System.Collections.Generic;
using Website.Core.DTO;

namespace Website.Web.Models.AdminViewModels
{
    public class ItemsViewModel
    {
        #region page state properties
        public string CurrentSearch { get; set; }
        public string CurrentSortOrder { get; set; }
        public bool Descending { get; set; }
        public int CurrentPage { get; set; }
        public int CountPerPage { get; set; }
        public int Types { get; set; }
        public int ItemCount { get; set; }
        #endregion

        public IEnumerable<ProductDto> Items { get; set; }
        public IEnumerable<CategoryDto> Categories { get; set; }
    }
}
