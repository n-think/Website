using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Website.Data.EF.Models;
using Website.Service.DTO;

namespace Website.Service.Mapper
{
    class ClientProfileMapperProfile : Profile
    {
        public ClientProfileMapperProfile()
        {
            CreateMap<ClientProfile, ClientProfileDTO>(MemberList.Destination).ReverseMap();
        }
    }
}
