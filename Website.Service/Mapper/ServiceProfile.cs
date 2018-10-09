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
            //CreateMap<UserDTO, User>(MemberList.Destination);
            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<UserProfile, UserProfileDTO>().ReverseMap();

            CreateMap<Product, ProductDTO>(MemberList.Destination)
                .ForMember(x => x.Images, opt => opt.Ignore())
                .ForMember(x => x.CategoryName, opt => opt.Ignore()).ReverseMap();

           // CreateMap<ProductDTO, Product>(MemberList.Source);




            //CreateMap<UserClaim,Claim>()
            //    .ForMember(x=>x.Type, opt=>opt.MapFrom(x=>x.ClaimType))
            //    .ForMember(x => x.Value, opt => opt.MapFrom(x => x.ClaimValue))
            //    .ForAllOtherMembers(x=>x.Ignore());

            CreateMap<Task<User>, Task<UserDTO>>().ReverseMap();
            CreateMap<Task<Role>, Task<RoleDTO>>().ReverseMap();
            CreateMap<Task<UserProfile>, Task<UserProfileDTO>>().ReverseMap();
        }
    }
}
