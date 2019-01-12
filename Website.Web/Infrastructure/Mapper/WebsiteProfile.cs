using System.Collections.Generic;
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
                    opt => opt.MapFrom<ProductDescGroupDtoResolver>())
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(x => x.ProductToCategory.Select(y => y.Category)));
            CreateMap<ItemViewModel, Product>();

            CreateMap<Product, EditItemViewModel>()
                .ForMember(dest => dest.DescriptionGroups,
                    opt => opt.MapFrom<ProductDescGroupDtoResolver>())
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(x => x.ProductToCategory.Select(y => y.Category)));

            CreateMap<Category, CategoryDto>();
            CreateMap<EditItemViewModel, Product>()
                .ForMember(x => x.Images, opt => opt.Ignore())
                .ForMember(x => x.ProductToCategory, opt => opt.Ignore())
                .ForMember(x => x.Descriptions, opt => opt.Ignore());

            CreateMap<Image, ImageDto>();
            CreateMap<ImageDto, Image>()
                .ConvertUsing<ImageDtoToImageConverter>();

            CreateMap<DescriptionGroupItemDto, Description>()
                .ConvertUsing<DescGroupItemDtoToDescConverter>();
        }
    }
}