using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Website.Data.EF.Models;
using Website.Service.DTO;
using Website.Service.Infrastructure;

namespace Website.Service.Interfaces
{
    public interface IClientService
    {
        Task<OperationDetails> CreateOrUpdateProfileAsync(ClientProfileDTO clientProfileDto);

        Task<IEnumerable<ClientDTO>> GetUsersAsync();
    }
}
