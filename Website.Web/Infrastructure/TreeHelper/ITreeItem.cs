using System.Collections.Generic;

namespace Website.Web.Infrastructure.TreeHelper
{
    public interface ITreeItem<T>
    {
        List<T> Children { get; set; }
    }
}