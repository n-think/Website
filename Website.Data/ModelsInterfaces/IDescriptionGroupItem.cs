using System.Collections.Generic;
using Website.Data.EF.Models;

namespace Website.Data.ModelsInterfaces
{
    public interface IDescriptionGroupItem
    {
        DescriptionGroup DescriptionGroup { get; set; }
        ICollection<Description> Descriptions { get; set; }
    }
}