using System.Threading.Tasks;
using Website.Core.Infrastructure;

namespace Website.Core.Interfaces.Services
{
    public interface IShopImageTransformer<T>
    {
        void ProcessImage(T imageData);
    }
}