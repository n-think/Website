using System.Collections.Generic;

namespace Website.Service.Infrastructure
{
    public class SortPageResult<T>
    {
        public IEnumerable<T> FilteredData { get; set; }
        public int TotalN { get; set; }
    }
}
