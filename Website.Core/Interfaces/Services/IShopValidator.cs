using System.Threading.Tasks;
using Website.Core.Infrastructure;

namespace Website.Core.Interfaces.Services
{
    public interface IShopValidator<T> where T : class
    {
        Task<OperationResult> ValidateAsync(IShopManager manager, T entity);
    }
}