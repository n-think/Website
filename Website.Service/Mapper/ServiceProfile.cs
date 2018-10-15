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
            CreateMap<User, UserDTO>(MemberList.Source).ReverseMap();
            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<UserProfile, UserProfileDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>(MemberList.Destination)
                .ForMember(x => x.Images, o => o.Ignore())
                .ReverseMap();
            CreateMap<Category, CategoryDTO>(MemberList.Destination).ReverseMap();

            CreateMap<Task<User>, Task<UserDTO>>().ReverseMap();
            CreateMap<Task<Role>, Task<RoleDTO>>().ReverseMap();
            CreateMap<Task<UserProfile>, Task<UserProfileDTO>>().ReverseMap();
        }
    }
}
