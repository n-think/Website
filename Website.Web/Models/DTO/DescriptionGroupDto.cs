using System.Collections.Generic;
using Website.Web.Infrastructure.TreeHelper;

namespace Website.Web.Models.DTO
{
    public class DescriptionGroupDto : ITreeItem<DescriptionGroupDto>
    {
        public DescriptionGroupDto()
        {
            Descriptions = new List<DescriptionDto>();
        }

        public int? Id { get; set; }
        public string Name { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual byte[] Timestamp { get; set; }

        public virtual DescriptionGroupDto Parent { get; set; }
        public List<DescriptionDto> Descriptions { get; set; }
        public List<DescriptionGroupDto> Children { get; set; }

        public override string ToString()
        {
            return $"Id:{Id}; Name:{Name}";
        }
    }
}
