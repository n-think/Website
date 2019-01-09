using System.Collections.Generic;

namespace Website.Web.Models.DTO
{
    public class DescriptionGroupDto
    {
        public DescriptionGroupDto()
        {
            Items = new List<DescriptionItemDto>();
        }

        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<DescriptionItemDto> Items { get; set; }

        public override string ToString()
        {
            return $"Id:{Id}; Name:{Name}; Desc:{Description}";
        }
    }
}
