using System.Threading;
using System.Threading.Tasks;

namespace Website.Core.Interfaces.Services
{
    public interface IUserProfileStore<TUserProfileDto> where TUserProfileDto : class
    {
        Task<TUserProfileDto> FindProfileByUserIdAsync(string userId, CancellationToken cancellationToken);
    }
}
