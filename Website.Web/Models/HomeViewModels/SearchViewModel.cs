using System.Collections.Generic;
using Website.Web.Models.DTO;

namespace Website.Web.Models.HomeViewModels
{
    public class SearchViewModel
    {
        #region page state properties

        public string CurrentSearch { get; set; }
        public string CurrentSortOrder { get; set; }
        public bool Descending { get; set; }
        public int CurrentPage { get; set; }
        public int CountPerPage { get; set; }
        public int ItemCount { get; set; }
        public int[] CategoryIds { get; set; }

        #endregion

        public IEnumerable<ProductDto> FilteredItems { get; set; }
        public IEnumerable<CategoryDto> AllCategories { get; set; }
        public IEnumerable<CategoryDto> FilteredCategories { get; set; }
    }
}