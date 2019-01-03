using System.Collections.Generic;
using Website.Core.Models;
using Website.Core.Models.Domain;

namespace Website.Core.Interfaces.Models
{
    public interface IDescriptionGroupItem
    {
        DescriptionGroup DescriptionGroup { get; set; }
        ICollection<Description> Descriptions { get; set; }
    }
}