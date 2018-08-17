using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Website.Web.TagHelpers
{
    public class PageLinkTagHelper : TagHelper
    {
        public string Property { get; set; } // значение текущего свойства, для которого создается тег
        public string Current { get; set; } // значение активного свойства, выбранного для сортировки
        public string Action { get; set; } // действие контроллера, на которое создается ссылка
        public bool Descending { get; set; } // сортировка по возрастанию или убыванию
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //TODO разбить
            
            string linkProp = Property;

            if (Descending)
            {
                Property += "_desc";
            }

            // если текущее свойство имеет значение CurrentSort
            if (Current == Property)
            {
                TagBuilder tag = new TagBuilder("i");
                tag.AddCssClass("glyphicon");
                if (Descending)// если сортировка по убыванию
                {
                    tag.AddCssClass("glyphicon-chevron-up");
                }
                else // если сортировка по возрастанию
                {
                    tag.AddCssClass("glyphicon-chevron-down");
                    linkProp += "_desc";
                }
                output.PostContent.Append(" "); // добавить пробел чтобы стерлка не была вплотную
                output.PostContent.AppendHtml(tag);
            }

            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "a";

            string url = urlHelper.Action(Action, new { sort = linkProp });
            output.Attributes.SetAttribute("href", url);
        }
    }
}
