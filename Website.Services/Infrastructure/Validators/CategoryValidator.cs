using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;

namespace Website.Services.Infrastructure.Validators
{
    public class CategoryValidator : IShopValidator<Category>
    {
        public async Task<OperationResult> ValidateAsync(IShopManager manager, Category entity)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var errors = new List<OperationError>();

            if (entity.Id < 0 || entity.Id == entity.ParentId)
            {
                errors.Add(manager.ErrorDescriber.InvalidModel());
            }
            
            if (entity.Name.IsNullOrEmpty())
            {
                errors.Add(manager.ErrorDescriber.EmptyCategoryName());
            }
            else
            {
                Category existing = await manager.GetCategoryByNameAsync(entity.Name);
                if (existing != null & existing?.Id != entity.Id)
                    errors.Add(manager.ErrorDescriber.DuplicateCategoryName());
            }
            
            if (errors.Count > 0)
            {
                return OperationResult.Failure(errors.ToArray());
            }

            return OperationResult.Success();
        }
    }
}