using System.Collections.Generic;
using AutoMapper;
using Website.Core.Models.Domain;
using Website.Web.Models.DTO;

namespace Website.Web.Infrastructure.Mapper
{
    public class DescGroupItemDtoToDescConverter : ITypeConverter<DescriptionGroupItemDto,Description>
    {
        public Description Convert(DescriptionGroupItemDto source, Description destination, ResolutionContext context)
        {
            return new Description()
            {
                Id = source.Id.GetValueOrDefault(),
                Value = source.DescriptionValue,
                ProductId = source.ProductId,
                DescriptionGroupItemId = source.Id
            };
        }
    }
}