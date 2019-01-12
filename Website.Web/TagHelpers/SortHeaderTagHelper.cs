using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Website.Web.TagHelpers
{
    [HtmlTargetElement("sort-header", TagStructure = TagStructure.WithoutEndTag)]
    public class SortHeaderTagHelper : TagHelper
    {
        public string Property { get; set; } // значение текущего свойства, для которого создается тег
        public string Current { get; set; } // значение активного свойства, выбранного для сортировки
        public string Action { get; set; } // действие контроллера, на которое создается ссылка
        public bool Descending { get; set; } // сортировка по возрастанию или убыванию
        public string Search { get; set; } // значение активного поиска
        public int Selector { get; set; } // значение выбранного селектора поиска
        public string Content { get; set; }// содержимое для отображения

        private IUrlHelperFactory _urlHelperFactory;

        public SortHeaderTagHelper(IUrlHelperFactory helperFactory)
        {
            _urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string linkProp = Property;

            if (Descending)
            {
                Property += "_desc";
            }

            var tag = new TagBuilder("i");
            tag.AddCssClass("fa");
            if (Current == Property)
            {
                if (Descending)// если сортировка по убыванию
                {
                    tag.AddCssClass("fa-arrow-up");
                }
                else // если сортировка по возрастанию
                {
                    tag.AddCssClass("fa-arrow-down");
                    linkProp += "_desc";
                }

            }
            else
            {
                tag.AddCssClass("fa-sort");
            }

            output.Content.SetContent(Content);
            output.PostContent.AppendHtml(" "); // добавить пробел чтобы стерлка не была вплотную
            output.PostContent.AppendHtml(tag);

            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "a";
            output.TagMode = TagMode.StartTagAndEndTag;

            string url = urlHelper.Action(Action, new { s = Search, st = Selector, o = linkProp }); // o - sort order query
            output.Attributes.SetAttribute("href", url);
        }
    }
}
