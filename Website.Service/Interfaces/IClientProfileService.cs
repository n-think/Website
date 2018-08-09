using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Website.Service.DTO;
using Website.Service.Infrastructure;

namespace Website.Service.Interfaces
{
    public interface IClientProfileService
    {
        Task<OperationDetails> CreateOrUpdate(ClientProfileDTO clientProfileDto);
        Task<string> GetFullName(string email);
    }
}
