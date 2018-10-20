using AutoMapper;
using Website.Service.DTO;
using Website.Web.Models.AdminViewModels;

namespace Website.Web.Infrastructure.Mapper
{
    public class WebsiteProfile : Profile
    {
        public WebsiteProfile()
        {
            CreateMap<UserDTO, UserViewModel>().ReverseMap();
            CreateMap<UserDTO, EditUserViewModel>().ReverseMap();

            CreateMap<ProductDTO, EditItemViewModel>()
                .ForMember(x => x.AllCategories, opt => opt.Ignore())
                .ForMember(x => x.AllDescriptionGroups, opt => opt.Ignore())
                .ForMember(x => x.JsonData, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
