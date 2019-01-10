using Website.Core.Enums;

namespace Website.Web.Models.DTO
{
    public class DescriptionDto
    {
        public int Id { get; set; }
        public int DescriptionGroupId { get; set; }
        public int ProductId { get; set; }
        public string Value { get; set; }
        public DtoState DtoState { get; set; }

        public override string ToString()
        {
            return $"Id:{Id}; Value:{Value}";
        }
    }
}
