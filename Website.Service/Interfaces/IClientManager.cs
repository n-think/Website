using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Website.Data.EF.Models;
using Website.Service.DTO;
using Website.Service.Enums;
using Website.Service.Infrastructure;

namespace Website.Service.Interfaces
{
    public interface IClientManager
    {
        Task<OperationDetails> CreateOrUpdateProfileAsync(ClientProfileDTO clientProfileDto);

        Task<ICollection<ClientDTO>> GetUsersAsync(RoleSelector rolePick, int skip, int take);
        Task <ICollection<ClientDTO>> GetSortFilterPageAsync(string sortOrder, string currentFilter, string searchString, int? page, int? count);
        Task LogUserActivity(string userLogin);
    }
}
