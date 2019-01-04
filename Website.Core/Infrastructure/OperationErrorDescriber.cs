﻿namespace Website.Core.Infrastructure
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

        public virtual OperationError InvalidModel()
        {
            return new OperationError()
            {
                Code = nameof(InvalidModel),
                Description = $"Получены некорректные данные."
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

        public virtual OperationError InvalidImageFormat()
        {
            return new OperationError
            {
                Code = nameof(InvalidImageFormat),
                Description = $"Изменения не сохранены. Некорректные одно или несколько изображений. Допустимые форматы: jpg, gif, png, tiff, bmp. Размер не более ~3 МБ"
            };
        }

        public virtual OperationError DiskIOError()
        {
            return new OperationError
            {
                Code = nameof(DiskIOError),
                Description = $"Возникла ошибка при сохранении изображений на диск."
            };
        }
        public virtual OperationError CannotDeleteActiveProduct()
        {
            return new OperationError
            {
                Code = nameof(CannotDeleteActiveProduct),
                Description = $"Нельзя удалить активный товар из магазина."
            };
        }
        public virtual OperationError ErrorDeletingProduct()
        {
            return new OperationError
            {
                Code = nameof(ErrorDeletingProduct),
                Description = $"Возникла ошибка при удалении товара."
            };
        }
    }
}