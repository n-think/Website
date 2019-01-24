using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Website.Core.Models.Domain;
using Website.Web.Infrastructure.TreeHelper;
using Website.Web.Models.HomeViewModels;
using Website.Web.Models.AdminViewModels;
using Website.Web.Models.DTO;
using Website.Web.Models.ManageViewModels;

namespace Website.Web.Infrastructure.Mapper
{
    public class WebsiteProfile : Profile
    {
        public WebsiteProfile()
        {
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<User, EditUserViewModel>().ReverseMap();
            CreateMap<User, DeleteUserViewModel>();
            CreateMap<User, ProfileViewModel>();
            CreateMap<ProfileViewModel, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Product, ProductDto>()
                .ReverseMap();
            CreateMap<Product, Models.AdminViewModels.ItemViewModel>()
                .ForMember(dest => dest.DescriptionGroups,
                    opt => opt.MapFrom<ProductDescGroupDtoResolver>())
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(x => x.ProductToCategory.Select(y => y.Category)));
            CreateMap<Models.AdminViewModels.ItemViewModel, Product>();
            CreateMap<Product, EditItemViewModel>()
                .ForMember(dest => dest.DescriptionGroups,
                    opt => opt.MapFrom<ProductDescGroupDtoResolver>())
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(x => x.ProductToCategory.Select(y => y.Category)));
            CreateMap<EditItemViewModel, Product>()
                .ForMember(x => x.Images, opt => opt.Ignore())
                .ForMember(x => x.ProductToCategory, opt => opt.Ignore())
                .ForMember(x => x.Descriptions, opt => opt.Ignore());
            CreateMap<Product, DeleteItemViewModel>();

            CreateMap<Category, CategoryDto>().ReverseMap();
            
            CreateMap<DescriptionGroup, DescriptionGroupDto>().ReverseMap();
            
            CreateMap<DescriptionGroupItem, DescriptionGroupItemDto>().ReverseMap();
            
            CreateMap<Product, Models.HomeViewModels.ItemViewModel>()
                .ForMember(dest => dest.DescriptionGroups,
                    opt => opt.MapFrom<ProductDescGroupDtoResolver>())
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(x => x.ProductToCategory.Select(y => y.Category)));
            
            CreateMap<Image, ImageDto>();
            CreateMap<ImageDto, Image>()
                .ConvertUsing<ImageDtoToImageConverter>();

            CreateMap<DescriptionGroupItemDto, Description>()
                .ConvertUsing<DescGroupItemDtoToDescConverter>();
        }
    }
}