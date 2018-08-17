using System;
using System.Collections.Generic;
using System.Text;

namespace Website.Service.Infrastructure
{
    public class SortPageResult<T>
    {
        public IEnumerable<T> FilteredData { get; set; }
        public int TotalN { get; set; }
    }
}
