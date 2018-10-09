using System;
using System.Collections.Generic;
using System.Text;

namespace Website.Service.Infrastructure
{
    public class StoreErrorDescriber
    {
        public virtual StoreError DefaultError()
        {
            return new StoreError()
            {
                Code = nameof(DefaultError),
                Description = $"Возникла непредвиденная ошибка."
            };
        }
    }
}
