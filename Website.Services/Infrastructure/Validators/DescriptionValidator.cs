using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;

namespace Website.Services.Infrastructure.Validators
{
    public class DescriptionValidator : IShopValidator<Description>
    {
        public Task<OperationResult> ValidateAsync(IShopManager manager, Description description)
        {
            if (description.Value.IsNullOrEmpty())
            {
                return Task.FromResult(OperationResult.Failure(manager.ErrorDescriber.EmptyProductDescriptionItem()));
            }

            if (!description.DescriptionGroupItemId.HasValue)
            {
                return Task.FromResult(OperationResult.Failure(manager.ErrorDescriber.InvalidModel("Описание без ID")));
            }
            
            return Task.FromResult(OperationResult.Success());
        }
    }
}