using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.Extensions.Options;
using Website.Core.Models.Domain;
using Website.Web.Models.DTO;

namespace Website.Web.Infrastructure.Mapper
{
    public class ImageDtoToImageConverter : ITypeConverter<ImageDto,Image>
    {
        public Image Convert(ImageDto source, Image destination, ResolutionContext context)
        {
            var image = new Image()
            {
                Id = source.Id.GetValueOrDefault(),
                Primary = source.Primary
            };
            if (source.DataUrl != null)
            {
                var data = source.DataUrl;
                string mimeType = data.Substring(data.IndexOf(':') + 1, data.IndexOf(';') - data.IndexOf(':') - 1);
                string base64 = data.Substring(data.IndexOf(',') + 1);
                var binArray = System.Convert.FromBase64String(base64);
                image.BinData = new ImageBinData()
                {
                    ImageId = source.Id.GetValueOrDefault(),
                    FullData = binArray,
                    ThumbData = binArray
                };
                image.Mime = mimeType;
            }
            return image;
        }
    }
}