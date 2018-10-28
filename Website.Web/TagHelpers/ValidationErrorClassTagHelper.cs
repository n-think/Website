using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Website.Web.TagHelpers
{
    [HtmlTargetElement("input", Attributes = "validation-for,validation-error-class,validation-valid-class")]
    public class ValidationErrorClassTagHelper : TagHelper
    {
        [HtmlAttributeName("validation-for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("validation-error-class")]
        public string ErrorClass { get; set; }
        [HtmlAttributeName("validation-valid-class")]
        public string ValidClass { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!ViewContext.ViewData.ModelState.IsValid)
            {
                ViewContext.ViewData.ModelState.TryGetValue(For?.Name ?? "", out ModelStateEntry entry);
                if (entry != null && entry.Errors.Any())
                {
                    output.AddClass(ErrorClass, HtmlEncoder.Default);
                }
                else if (entry != null)
                {
                    output.AddClass(ValidClass, HtmlEncoder.Default);
                }
            }
        }
    }
}
