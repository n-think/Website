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
    [HtmlTargetElement("page-link", TagStructure = TagStructure.WithoutEndTag)]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public string Action { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsCount { get; set; }
        public int ItemsPerPage { get; set; }
        public string CurrentProp { get; set; } // значение активного свойства, выбранного для сортировки
        public string Search { get; set; } // значение активного поиска
        public int Selector { get; set; } // значение выбранного селектора поиска

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            var totalPages = (int)Math.Ceiling((double)ItemsCount / ItemsPerPage);

            // набор ссылок будет представлять список ul
            TagBuilder tag = new TagBuilder("ul");
            tag.AddCssClass("pagination");
            tag.AddCssClass($"{Action}-pagination");

            var maxCont = 10; // максимальное количество страниц пока нумерация не рвется
            if (totalPages > maxCont)
            {
                for (var i = 1; i <= maxCont / 2; i++)
                {
                    var item = CreateTag(i, urlHelper);
                    tag.InnerHtml.AppendHtml(item);
                }

                var dotted = new TagBuilder("li");
                var link = new TagBuilder("a");
                link.InnerHtml.Append(" .... ");
                link.AddCssClass("dots");
                dotted.InnerHtml.AppendHtml(link);
                tag.InnerHtml.AppendHtml(dotted);

                for (var i = totalPages + 1 - maxCont / 2; i <= totalPages; i++)
                {
                    var item = CreateTag(i, urlHelper);
                    tag.InnerHtml.AppendHtml(item);
                }
            }
            else
            {
                for (var i = 1; i <= totalPages; i++)
                {
                    var item = CreateTag(i, urlHelper);
                    tag.InnerHtml.AppendHtml(item);
                }
            }

            output.PostContent.AppendHtml(tag);
        }

        TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper)
        {
            TagBuilder item = new TagBuilder("li");
            TagBuilder link = new TagBuilder("a");
            if (pageNumber == this.CurrentPage)
            {
                item.AddCssClass("active");
            }
            else
            {
                link.Attributes["href"] = urlHelper.Action(Action, new { s = Search, st = Selector, o = CurrentProp, p = pageNumber, c = ItemsPerPage });
            }
            link.InnerHtml.Append(pageNumber.ToString());
            item.InnerHtml.AppendHtml(link);
            return item;
        }

    }
}
