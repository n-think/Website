using AutoMapper;
using Website.Service.DTO;
using Website.Web.Models.AdminViewModels;

namespace Website.Web.Infrastructure.Mapper
{
    public class WebsiteProfile : Profile
    {
        public WebsiteProfile()
        {
            CreateMap<UserDto, UserViewModel>().ReverseMap();
            CreateMap<UserDto, EditUserViewModel>().ReverseMap();

            CreateMap<ProductDto, EditItemViewModel>()
                .ForMember(x => x.AllCategories, opt => opt.Ignore())
                .ForMember(x => x.AllDescriptionGroups, opt => opt.Ignore())
                .ForMember(x => x.JsonData, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
