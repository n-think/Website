using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Website.Service.DTO;

namespace Website.Service.Interfaces
{
    interface IUserProfileStore<TUserProfileDto> where TUserProfileDto : class
    {
        Task<TUserProfileDto> FindProfileByUserIdAsync(string userId, CancellationToken cancellationToken);
    }
}
