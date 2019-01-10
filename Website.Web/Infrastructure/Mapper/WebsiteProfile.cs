using System.Linq;
using AutoMapper;
using Website.Core.Models.Domain;
using Website.Web.Infrastructure.TreeHelper;
using Website.Web.Models.AdminViewModels;
using Website.Web.Models.DTO;

namespace Website.Web.Infrastructure.Mapper
{
    public class WebsiteProfile : Profile
    {
        public WebsiteProfile()
        {
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<User, EditUserViewModel>().ReverseMap();

            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, ItemViewModel>()
                .ForMember(dest => dest.DescriptionGroups,
                    opt => opt.MapFrom(x => x.Descriptions.Select(y => y.DescriptionGroup).ToList()))
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(x => x.ProductToCategory.Select(y => y.Category)))
                //.AfterMap((src, dest) => dest.DescriptionGroups = dest.DescriptionGroups.ToTree()) дерево уже собрано ef'ом
                .AfterMap((src, dest) => dest.DescriptionGroups = dest.DescriptionGroups
                    .Where(x => x.ParentId == null)
                    .ToList());// взять только корни

            CreateMap<Product, EditItemViewModel>();

            CreateMap<Description, DescriptionDto>();
            CreateMap<Category, CategoryDto>();
        }
    }
}