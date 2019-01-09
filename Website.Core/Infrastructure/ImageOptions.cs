using System.Drawing.Imaging;

namespace Website.Core.Infrastructure
{
    public class ImageOptions
    {
        public ImageOptions()
        {
            SaveFormat = ImageFormat.Jpeg;
            MaxHeight = 1000;
            MaxWidth = 1000;
            MaxThumbHeight = 150;
            MaxThumbWidth = 150;
            EncoderQuality = 80L;
        }

        public ImageFormat SaveFormat { get; set; }
        public short MaxHeight { get; set; }
        public short MaxWidth { get; set; }
        public short MaxThumbHeight { get; set; }
        public short MaxThumbWidth { get; set; }
        public long EncoderQuality { get; set; }

    }
}