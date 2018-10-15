using System;
using System.Collections.Generic;
using System.Text;

namespace Website.Service.Infrastructure
{
    public class OperationErrorDescriber
    {
        public virtual OperationError DefaultError()
        {
            return new OperationError()
            {
                Code = nameof(DefaultError),
                Description = $"Возникла непредвиденная ошибка."
            };
        }

        public virtual OperationError ConcurrencyFailure()
        {
            return new OperationError
            {
                Code = nameof(ConcurrencyFailure),
                Description = $"Запись над которой вы работали была изменена другим пользователем. Изменения не были сохранены. Проверьте данные перед повторной отправкой."
            };
        }

        public virtual OperationError DbUpdateFailure()
        {
            return new OperationError
            {
                Code = nameof(DbUpdateFailure),
                Description = $"Возникла ошибка обновления базы данных. Изменения не были сохранены."
            };
        }
    }
}
