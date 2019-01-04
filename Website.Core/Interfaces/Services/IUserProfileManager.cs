using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Website.Core.DTO;
using Website.Core.Enums;
using Website.Core.Infrastructure;

namespace Website.Core.Interfaces.Services
{
    public interface IUserProfileManager
    {
        Task<OperationResult> CreateOrUpdateProfileAsync(UserProfileDto userProfileDto);
        Task<UserProfileDto> FindProfileByUserIdAsync(string userId);
    }
}
