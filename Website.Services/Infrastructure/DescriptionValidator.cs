using System.Threading.Tasks;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;

namespace Website.Services.Infrastructure
{
    public class DescriptionValidator : IShopValidator<Description>
    {
        public async Task<OperationResult> ValidateAsync(IShopManager manager, Description entity)
        {
            throw new System.NotImplementedException();
        }
    }
}