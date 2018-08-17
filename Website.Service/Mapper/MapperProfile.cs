using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Website.Data.EF.Models;
using Website.Service.DTO;

namespace Website.Service.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserProfile, UserProfileDTO>().ReverseMap();
            CreateMap<User, UserDTO>(MemberList.Source).ForSourceMember(x=>x.Id , opt=>opt.Ignore()).ReverseMap();
            CreateMap<Role, RoleDTO>().ReverseMap();

            //CreateMap<List<User>, List<UserDTO>>().ReverseMap();
            //CreateMap<UserRole<string>, UserRoleDTO>().ReverseMap();


            CreateMap<Task<User>, Task<UserDTO>>(MemberList.None).ForSourceMember(x => x.Id, opt => opt.Ignore()).ReverseMap();
            CreateMap<Task<Role>, Task<RoleDTO>>().ReverseMap();
            //CreateMap<Task<UserRole<string>>, Task<UserRoleDTO>>().ReverseMap();
        }
    }
}
