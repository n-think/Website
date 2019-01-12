using System.Threading.Tasks;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;

namespace Website.Services.Infrastructure.Validators
{
    public class DescriptionGroupValidator : IShopValidator<DescriptionGroup>
    {
        public async Task<OperationResult> ValidateAsync(IShopManager manager, DescriptionGroup entity)
        {
            throw new System.NotImplementedException();
        }
    }
}