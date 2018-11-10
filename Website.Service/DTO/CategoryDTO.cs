using Website.Service.Enums;

namespace Website.Service.DTO
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentId { get; set; }
        public byte[] Timestamp { get; set; }
        public int? ProductCount { get; set; }
        public DtoState DtoState { get; set; }
    }
}
