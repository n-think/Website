using AutoMapper;
using Website.Web.Models.AdminViewModels;
using Website.Web.Models.DTO;

namespace Website.Web.Infrastructure.Mapper
{
    public class WebsiteProfile : Profile
    {
        public WebsiteProfile()
        {
            CreateMap<UserDto, UserViewModel>().ReverseMap();
            CreateMap<UserDto, EditUserViewModel>().ReverseMap();

            CreateMap<ProductDto, EditItemViewModel>()
                .ReverseMap();
        }
    }
}
