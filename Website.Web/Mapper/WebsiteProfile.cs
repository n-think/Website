using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoMapper;
using Website.Service.DTO;
using Website.Web.Models.AdminViewModels;

namespace Website.Web.Mapper
{
    public class WebsiteProfile : Profile
    {
        public WebsiteProfile()
        {
            CreateMap<UserDTO, UserViewModel>().ReverseMap();
            CreateMap<UserDTO, EditUserViewModel>().ReverseMap();
        }
    }
}
