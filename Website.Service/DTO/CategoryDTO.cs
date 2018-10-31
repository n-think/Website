namespace Website.Service.DTO
{
    public class CategoryDto
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public int? ParentId { get; set; }
        public byte[] Timestamp { get; set; }
    }
}
