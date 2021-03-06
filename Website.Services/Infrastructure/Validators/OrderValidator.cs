using System;
using System.Threading.Tasks;
using Website.Core.Infrastructure;
using Website.Core.Interfaces.Services;
using Website.Core.Models.Domain;

namespace Website.Services.Infrastructure.Validators
{
    public class OrderValidator : IShopValidator<Order>
    {
        public/* async*/ Task<OperationResult> ValidateAsync(IShopManager manager, Order entity)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            throw new System.NotImplementedException();
        }
    }
}