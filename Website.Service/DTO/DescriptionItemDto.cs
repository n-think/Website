using Website.Service.Enums;

namespace Website.Service.DTO
{
    public class DescriptionItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? DescriptionGroupId { get; set; }
        public int? ProductId { get; set; }
        public int? DescriptionId { get; set; }
        public string DescriptionValue { get; set; }
        public DtoState DtoState { get; set; }

        public override string ToString()
        {
            return $"Id:{Id}; ProductId:{ProductId}; Name:{Name}; Value:{DescriptionValue}";
        }
    }
}
