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
    public interface ICustomUserManager
    {
        Task<OperationDetails> CreateOrUpdateProfileAsync(UserProfileDTO userProfileDto);

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <param name="rolePick"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        Task<ICollection<UserDTO>> GetUsersAsync(RoleSelector rolePick, int skip, int take);
        Task <ICollection<UserDTO>> GetSortFilterPageAsync(string sortOrder, string currentFilter, string searchString, int? page, int? count);
        Task LogUserActivity(string userLogin);
    }
}
