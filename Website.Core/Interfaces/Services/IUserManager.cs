using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Website.Core.Enums;
using Website.Core.Infrastructure;
using Website.Core.Models.Domain;

namespace Website.Core.Interfaces.Services
{
    public interface IUserManager : IIdentityUserManager<User>
    {
        Task<SortPageResult<User>> GetSortFilterPageAsync(RoleSelector roleSelector, string searchString, string sortOrder, int page, int count);
        Task UpdateUserLastActivityDate(int userId);
        Task<IdentityResult> UpdateUserPasswordClaims(User user, string newPassword, IEnumerable<Claim> claims);

    }
}
