using System.Threading.Tasks;
using Website.Core.Infrastructure;

namespace Website.Core.Interfaces.Services
{
    public interface IDatabaseInitializer
    {
        Task<OperationResult> InitializeAdminAccountRolesAsync();
        Task<int> GenerateUsersAsync(int count);
        Task<int> GenerateItemsAsync(int count);
        Task<OperationResult> DropCreateDatabase();
    }
}