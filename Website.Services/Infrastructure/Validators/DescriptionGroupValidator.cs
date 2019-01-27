using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;

namespace Website.Services.Infrastructure.Validators
{
    public class DescriptionGroupValidator : IShopValidator<DescriptionGroup>
    {
        public async Task<OperationResult> ValidateAsync(IShopManager manager, DescriptionGroup entity)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            var errors = new List<OperationError>();
            
            if (entity.Name.IsNullOrEmpty())
            {
                errors.Add(manager.ErrorDescriber.EmptyDescriptionGroupName());
            }
            else
            {
                var existing = await manager.GetDescriptionGroupByNameAsync(entity.Name);
                if (existing != null && existing?.Id != entity.Id)
                    errors.Add(manager.ErrorDescriber.DuplicateDescriptionGroupName());
            }
            
            if (errors.Count > 0)
            {
                return OperationResult.Failure(errors.ToArray());
            }

            return OperationResult.Success();
        }
    }
}