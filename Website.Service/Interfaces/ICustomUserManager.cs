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
        /// Skips <paramref name="skipCount"/> and takes next <paramref name="takeCount"/> users in <see cref="RoleSelector"/> roles.
        /// </summary>
        /// <param name="rolePick">Select role type</param>
        /// <param name="skipCount">Users to skip</param>
        /// <param name="takeCount">Users to take</param>
        /// <returns></returns>
        Task<IEnumerable<UserDTO>> GetUsersAsync(RoleSelector rolePick, int skipCount, int takeCount);
        Task<SortPageResult<UserDTO>> GetSortFilterPageAsync(RoleSelector roleSelector, string searchString, string sortOrder, int page, int count);
        Task LogUserActivity(string userLogin);
    }
}
