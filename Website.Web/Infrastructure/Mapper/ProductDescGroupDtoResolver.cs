using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Castle.Core.Internal;
using Website.Core.Models.Domain;
using Website.Web.Models.AdminViewModels;
using Website.Web.Models.DTO;

namespace Website.Web.Infrastructure.Mapper
{
    public class ProductDescGroupDtoResolver : IValueResolver<Product, ItemViewModel, List<DescriptionGroupDto>>,
        IValueResolver<Product, EditItemViewModel, List<DescriptionGroupDto>>
    {
        public List<DescriptionGroupDto> Resolve(Product source, ItemViewModel destination, List<DescriptionGroupDto> destMember, ResolutionContext context)
        {
            return ConvertDescsToDescGroupDto(source);
        }

        public List<DescriptionGroupDto> Resolve(Product source, EditItemViewModel destination, List<DescriptionGroupDto> destMember, ResolutionContext context)
        {
            return ConvertDescsToDescGroupDto(source);
        }

        private static List<DescriptionGroupDto> ConvertDescsToDescGroupDto(Product source)
        {
            var descs = source.Descriptions.ToList(); 

            if (descs.IsNullOrEmpty())
                return null;

            descs.ForEach(x =>
            {
                if (x.DescriptionGroupItem.DescriptionGroup == null)
                    x.DescriptionGroupItem.DescriptionGroup = new DescriptionGroup()
                        {Id = -1, Name = "*группа удалена*"};
            });

            var descGroups = descs //get flattened description groups from description nav property
                .Select(x => x.DescriptionGroupItem.DescriptionGroup)
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .Select(x => new DescriptionGroupDto() //convert description groups to dto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                })
                .OrderBy(x => x.Name != "Общие характеристики") //ordering
                .ThenBy(x => x.Name)
                .ToList();

            var descItems = descs //convert descriptions to dto items
                .OrderBy(x => x.DescriptionGroupItem.Name) //ordering
                .Select(x =>
                {
                    var dto = new DescriptionGroupItemDto()
                    {
                        Id = x.DescriptionGroupItem.Id,
                        Name = x.DescriptionGroupItem.Name,
                        DescriptionGroupId = x.DescriptionGroupItem.DescriptionGroup.Id,
                        DescriptionId = x.Id,
                        DescriptionValue = x.Value
                    };
                    if (x.DescriptionGroupItem.DescriptionGroup.Name == "*группа удалена*" ||
                        x.DescriptionGroupItem.DescriptionGroup.Id == -1)
                        dto.Id = null;
                    return dto;
                })
                .ToList();
            
            //add descriptions to their respective groups
            foreach (var descGroup in descGroups)
            {
                foreach (var descItem in descItems)
                {
                    if (descItem.DescriptionGroupId == descGroup.Id)
                    {
                        descGroup.DescriptionItems.Add(descItem);
                    }
                }
            }

            return descGroups;
        }
    }
}