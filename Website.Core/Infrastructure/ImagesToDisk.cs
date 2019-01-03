using System.Collections.Generic;
using System.Drawing;

namespace Website.Core.Infrastructure
{
    public class ImagesToDisk
    {
        public ImagesToDisk()
        {
            ImagesToSave = new Dictionary<string, Bitmap>();
            ImagesAndThumbsToDelete = new List<(string, string)>();
        }
        public Dictionary<string, Bitmap> ImagesToSave { get; set; }
        public List<(string, string)> ImagesAndThumbsToDelete { get; set; }
    }
}
