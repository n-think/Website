namespace Website.Core.Models.Domain
{
    public class ImageBinData
    {
        public virtual int ImageId { get; set; }
        public virtual byte[] FullData { get; set; }
        public virtual byte[] ThumbData { get; set; }
        
        public virtual Image ImageInfo { get; set; }
    }
}