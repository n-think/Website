using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;



//локализация атрибутов ошибок , перехватывает итоговые сообщения ошибок


// не используется, если юзать добавить  в стартап


//services.AddSingleton<Microsoft.AspNetCore.Mvc.DataAnnotations.IValidationAttributeAdapterProvider, LocalizedValidationAttributeAdapterProvider>();





namespace Website.Web.Localization
{
    public class LocalizedValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        private readonly ValidationAttributeAdapterProvider _originalProvider = new ValidationAttributeAdapterProvider();

        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            attribute.ErrorMessage = attribute.GetType().Name.Replace("Attribute", string.Empty);
            if (attribute is DataTypeAttribute dataTypeAttribute)
                attribute.ErrorMessage += "_" + dataTypeAttribute.DataType;

            return _originalProvider.GetAttributeAdapter(attribute, stringLocalizer);
        }
    }
}
