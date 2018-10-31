using System.Threading;
using System.Threading.Tasks;

namespace Website.Service.Interfaces
{
    interface IUserProfileStore<TUserProfileDto> where TUserProfileDto : class
    {
        Task<TUserProfileDto> FindProfileByUserIdAsync(string userId, CancellationToken cancellationToken);
    }
}
