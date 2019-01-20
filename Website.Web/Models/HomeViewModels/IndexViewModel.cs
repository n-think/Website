using System.Collections.Generic;
using Website.Web.Models.DTO;

namespace Website.Web.Models.HomeViewModels
{
    public class IndexViewModel
    {
        #region page state properties
        public int CurrentPage { get; set; }
        public int CountPerPage { get; set; }
        public int ItemCount { get; set; }
        #endregion
        
        public IEnumerable<ProductDto> RecentItems { get; set; }
        public IEnumerable<ProductDto> Items { get; set; }
    }
}