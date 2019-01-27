namespace Website.Core.Infrastructure
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

        public virtual OperationError InvalidModel(string modelName = null)
        {
            return new OperationError()
            {
                Code = nameof(InvalidModel),
                Description = $"Получены некорректные данные." + (modelName == null ? "": $" {modelName}")
            };
        }

        public virtual OperationError ConcurrencyFailure()
        {
            return new OperationError
            {
                Code = nameof(ConcurrencyFailure),
                Description =
                    $"Запись над которой вы работали была изменена другим пользователем. Изменения не были сохранены. Проверьте данные перед повторной отправкой."
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
                Description =
                    $"Изменения не сохранены. Некорректные одно или несколько изображений. Допустимые форматы: jpg, gif, png, tiff, bmp."
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

        public virtual OperationError DuplicateProductCode()
        {
            return new OperationError
            {
                Code = nameof(DuplicateProductCode),
                Description = $"Продукт с таким артикулом уже существует."
            };
        }

        public virtual OperationError EmptyProductName()
        {
            return new OperationError
            {
                Code = nameof(EmptyProductName),
                Description = $"Наименование продукта не может быть пустым."
            };
        }

        public virtual OperationError InvalidProductId()
        {
            return new OperationError
            {
                Code = nameof(InvalidProductId),
                Description = $"Некорректный ID продукта."
            };
        }
        
        public virtual OperationError InvalidDescriptionItemId()
        {
            return new OperationError
            {
                Code = nameof(InvalidProductId),
                Description = $"Некорректный ID продукта."
            };
        }

        public virtual OperationError EmptyProductCode()
        {
            return new OperationError
            {
                Code = nameof(EmptyProductCode),
                Description = $"Необходимо указать артикул продукта."
            };
        }
        
        public virtual OperationError EmptyDescriptionGroupName()
        {
            return new OperationError
            {
                Code = nameof(EmptyDescriptionGroupName),
                Description = $"Имя группы описания не может быть пустым."
            };
        }
        
        public virtual OperationError EmptyDescriptionGroupItemName()
        {
            return new OperationError
            {
                Code = nameof(EmptyDescriptionGroupItemName),
                Description = $"Имя описания не может быть пустым."
            };
        }
        public virtual OperationError EmptyDescriptionValue()
        {
            return new OperationError
            {
                Code = nameof(EmptyDescriptionValue),
                Description = $"Описание не может быть пустым."
            };
        }

        public virtual OperationError EntityNotFound(string entityName)
        {
            return new OperationError
            {
                Code = nameof(EntityNotFound),
                Description = $"Сущность: \"{entityName}\" не найдена."
            };
        }

        public virtual OperationError CannotDeleteCategoryWithProducts()
        {
            return new OperationError
            {
                Code = nameof(CannotDeleteCategoryWithProducts),
                Description = $"Нельзя удалить категорию в которой находятся продукты."
            };
        }
        
        public virtual OperationError CannotDeleteDescGroupWithProducts()
        {
            return new OperationError
            {
                Code = nameof(CannotDeleteCategoryWithProducts),
                Description = $"Нельзя удалить группу в которой находятся активные описания продуктов."
            };
        }
        
        public virtual OperationError CannotDeleteDescItemsWithProducts()
        {
            return new OperationError
            {
                Code = nameof(CannotDeleteCategoryWithProducts),
                Description = $"Нельзя удалить описания которые используются продуктами."
            };
        }

        public virtual OperationError DuplicateDescriptionGroupName()
        {
            return new OperationError
            {
                Code = nameof(DuplicateDescriptionGroupName),
                Description = $"Группа с таким названием уже существует."
            };
        }
        
        public virtual OperationError DuplicateDescriptionGroupItemName()
        {
            return new OperationError
            {
                Code = nameof(DuplicateDescriptionGroupItemName),
                Description = $"Описание с таким названием уже существует."
            };
        }
        
        public virtual OperationError DuplicateCategoryName()
        {
            return new OperationError
            {
                Code = nameof(DuplicateCategoryName),
                Description = $"Категория с таким названием уже существует."
            };
        }
        
        public virtual OperationError EmptyCategoryName()
        {
            return new OperationError
            {
                Code = nameof(EmptyCategoryName),
                Description = $"Имя категории не может быть пустым."
            };
        }

        public virtual OperationError DuplicateProductName()
        {
            return new OperationError
            {
                Code = nameof(DuplicateProductName),
                Description = $"Товар с таким названием уже существует."
            };
        }
    }
}