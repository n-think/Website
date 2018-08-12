using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Website.Data.EF.Models;
using Website.Service.DTO;

namespace Website.Service.Mapper
{
    class ClientMapperProfile : Profile
    {
        public ClientMapperProfile()
        {
            CreateMap<ApplicationUser,ClientDTO>(MemberList.Destination)
                .ForMember(dest=> dest.ProfileDto, opt=>opt.MapFrom(src=>src.ClientProfile))
                .ForMember(dest => dest.UserLogin, opt => opt.MapFrom(src => src.UserName))
                .ReverseMap();
        }
    }
}
