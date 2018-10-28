namespace Website.Service.DTO
{
    public class DescriptionItemDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"Id:{Id}; Name:{Name}; Value:{Value}";
        }
    }
}
