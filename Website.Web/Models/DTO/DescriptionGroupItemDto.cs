using Website.Core.Enums;

namespace Website.Web.Models.DTO
{
    public class DescriptionGroupItemDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? DescriptionGroupId { get; set; }
        public int? ProductId { get; set; }
        public int? DescriptionId { get; set; }
        public string DescriptionValue { get; set; }
        public DtoState DtoState { get; set; }

        public override string ToString()
        {
            return $"Id:{Id}; Name:{Name}; Value:{DescriptionValue}";
        }
    }
}
