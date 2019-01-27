using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;

namespace Website.Services.Infrastructure.Validators
{
    public class DescriptionGroupItemValidator : IShopValidator<DescriptionGroupItem>
    {
        public async Task<OperationResult> ValidateAsync(IShopManager manager, DescriptionGroupItem entity)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            var errors = new List<OperationError>();

            if (entity.Name.IsNullOrEmpty())
            {
                errors.Add(manager.ErrorDescriber.EmptyDescriptionGroupItemName()); 
            }
            else
            {
                var existing = await manager.GetDescriptionItemByNameAsync(entity.Name);
                if (existing !=null && existing?.Id != entity.Id && existing.DescriptionGroupId == entity.DescriptionGroupId)
                    errors.Add(manager.ErrorDescriber.DuplicateDescriptionGroupItemName());
            }

            var descGroup = await manager.GetDescriptionGroupByIdAsync(entity.DescriptionGroupId.GetValueOrDefault());
            if (descGroup == null)
                errors.Add(manager.ErrorDescriber.EntityNotFound("Группа описаний"));
            
            if (errors.Count > 0)
            {
                return OperationResult.Failure(errors.ToArray());
            }

            return OperationResult.Success();
        }
    }
}