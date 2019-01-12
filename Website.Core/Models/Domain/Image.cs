namespace Website.Core.Models.Domain
{
    public class Image
    {
        public virtual int Id { get; set; }
        public virtual int? ProductId { get; set; }
        public virtual string Mime { get; set; }
        public virtual bool Primary { get; set; }


        public virtual ImageBinData BinData { get; set; }
        public virtual Product Product { get; set; }
    }
}
