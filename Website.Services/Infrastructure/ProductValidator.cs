using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;

namespace Website.Services.Infrastructure
{
    public class ProductValidator : IShopValidator<Product>
    {
        public ProductValidator(OperationErrorDescriber errorDescriber)
        {
            ErrorDescriber = errorDescriber ?? new OperationErrorDescriber();
        }

        private OperationErrorDescriber ErrorDescriber { get; set; }

        public async Task<OperationResult> ValidateAsync(IShopManager manager, Product product)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (product == null) throw new ArgumentNullException(nameof(product));

            var errors = new List<OperationError>();
            await ValidateProductAsync(manager, product, errors);
            if (errors.Count > 0)
            {
                return OperationResult.Failure(errors.ToArray());
            }

            return OperationResult.Success();
        }

        private async Task ValidateProductAsync(IShopManager manager, Product product,
            ICollection<OperationError> errors)
        {
            if (product.Name.IsNullOrEmpty())
            {
                errors.Add(ErrorDescriber.EmptyProductName());
            }
            
            if (product.Name.IsNullOrEmpty())
            {
                errors.Add(ErrorDescriber.EmptyProductName());
            }

            if (await manager.GetProductByCodeAsync(product.Code, false, false, false) != null)
            {
                errors.Add(ErrorDescriber.DuplicateProductCode());
            }
        }
    }
}