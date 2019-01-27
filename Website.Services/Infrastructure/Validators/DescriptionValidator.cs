using System;
using System.Collections.Generic;
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
        public async Task<OperationResult> ValidateAsync(IShopManager manager, Description description)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (description == null) throw new ArgumentNullException(nameof(description));
            
            var errors = new List<OperationError>();
            
            if (description.Value.IsNullOrEmpty())
            {
                errors.Add(manager.ErrorDescriber.EmptyDescriptionValue());
            }

            if (!description.DescriptionGroupItemId.HasValue)
            {
                errors.Add(manager.ErrorDescriber.InvalidModel());
            }
            
            var existingDescItem = await manager.GetDescriptionItemByIdAsync(description.DescriptionGroupItemId.GetValueOrDefault());
            if (existingDescItem == null)
            {
                errors.Add(manager.ErrorDescriber.EntityNotFound("Пункт описания группы"));
            }
            
            if (errors.Count > 0)
            {
                return OperationResult.Failure(errors.ToArray());
            }

            return OperationResult.Success();
        }
    }
}