using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;

namespace Website.Services.Infrastructure.Validators
{
    public class ProductValidator : IShopValidator<Product>
    {
        public async Task<OperationResult> ValidateAsync(IShopManager manager, Product product)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (product == null) throw new ArgumentNullException(nameof(product));

            var errors = new List<OperationError>();
            
            if (product.Name.IsNullOrEmpty())
            {
                errors.Add(manager.ErrorDescriber.EmptyProductName());
            }

            if (product.Code <= 0)
            {
                errors.Add(manager.ErrorDescriber.EmptyProductCode());
            }

            var prod = await manager.GetProductByCodeAsync(product.Code, false, false, false);
            if (prod != null && prod.Id != product.Id)
            {
                errors.Add(manager.ErrorDescriber.DuplicateProductCode());
            }

            prod = await manager.GetProductByNameAsync(product.Name, false, false, false);
            if (prod != null && prod.Id != product.Id)
            {
                errors.Add(manager.ErrorDescriber.DuplicateProductName());
            }
            
            if (errors.Count > 0)
            {
                return OperationResult.Failure(errors.ToArray());
            }

            return OperationResult.Success();
        }
    }
}