using AutoMapper;
using Website.Core.DTO;
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
                .ReverseMap();
        }
    }
}
