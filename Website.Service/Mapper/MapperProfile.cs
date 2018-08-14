using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Website.Data.EF.Models;
using Website.Service.DTO;

namespace Website.Service.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ApplicationUser,ClientDTO>(MemberList.Destination)
                .ForMember(dest=> dest.ProfileDto, opt=>opt.MapFrom(src=>src.ClientProfile))
                .ForMember(dest => dest.UserLogin, opt => opt.MapFrom(src => src.UserName))
                .ReverseMap();

            CreateMap<ClientProfile, ClientProfileDTO>(MemberList.Destination).ReverseMap();

            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<IdentityRole, RoleDTO>().ReverseMap();
            CreateMap<IdentityUserRole<string>, UserRoleDTO>().ReverseMap();

            CreateMap<Task<ApplicationUser>, Task<UserDTO>>().ReverseMap();
            CreateMap<Task<IdentityRole>, Task<RoleDTO>>().ReverseMap();
            CreateMap<Task<IdentityUserRole<string>>, Task<UserRoleDTO>>().ReverseMap();


        }
    }
}
