using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Website.Data.EF.Models;
using Website.Service.DTO;

namespace Website.Service.Mapper
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<User, UserDto>(MemberList.Source).ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<UserProfile, UserProfileDto>().ReverseMap();
            CreateMap<Product, ProductDto>(MemberList.Destination)
                .ForMember(x => x.Images, o => o.Ignore())
                .ForMember(x => x.Descriptions, o => o.Ignore())
                .ReverseMap();
            CreateMap<Category, CategoryDto>(MemberList.Destination).ReverseMap();

            CreateMap<Task<User>, Task<UserDto>>().ReverseMap();
            CreateMap<Task<Role>, Task<RoleDto>>().ReverseMap();
            CreateMap<Task<UserProfile>, Task<UserProfileDto>>().ReverseMap();
        }
    }
}
