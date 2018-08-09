using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

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
