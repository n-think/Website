using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Website.Web.TagHelpers
{
    [HtmlTargetElement("page-link", 
        TagStructure = TagStructure.WithoutEndTag)]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory _urlHelperFactory;
        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            _urlHelperFactory = helperFactory;
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
        
        public int[] Categories { get; set; }
        
        public int[] DescGroups { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            var totalPages = (int)Math.Ceiling((double)ItemsCount / ItemsPerPage);

            // набор ссылок будет представлять список ul
            TagBuilder tag = new TagBuilder("ul");
            tag.AddCssClass($"{Action.ToLower()}-pagination pagination justify-content-center");

            var maxCount = 5; // максимальное количество страниц пока нумерация не рвется
            if (totalPages > 2 + maxCount * 2)
            {
                var dotted = new TagBuilder("li");
                dotted.AddCssClass("page-item disabled");
                var link = new TagBuilder("a");
                link.InnerHtml.Append("...");
                link.AddCssClass("page-link");
                dotted.InnerHtml.AppendHtml(link);

                tag.InnerHtml.AppendHtml(CreateTag(1, urlHelper)); // всегда есть первый

                if (CurrentPage - maxCount - 1 > 0)
                    tag.InnerHtml.AppendHtml(dotted);

                for (var i = CurrentPage - maxCount + 1; i < CurrentPage; i++)
                {
                    if (i <= 1)
                        continue;
                    var item = CreateTag(i, urlHelper);
                    tag.InnerHtml.AppendHtml(item);
                }

                if (CurrentPage != 1 && CurrentPage != totalPages) // текущая, если она не первая и не последняя
                    tag.InnerHtml.AppendHtml(CreateTag(CurrentPage, urlHelper));

                for (var i = CurrentPage + 1; i < maxCount + CurrentPage && i < totalPages; i++)
                {
                    var item = CreateTag(i, urlHelper);
                    tag.InnerHtml.AppendHtml(item);
                }

                if (CurrentPage + maxCount < totalPages)
                    tag.InnerHtml.AppendHtml(dotted);

                tag.InnerHtml.AppendHtml(CreateTag(totalPages, urlHelper)); // всегда есть последний
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
            item.AddCssClass("page-item");
            TagBuilder link;

            if (pageNumber == this.CurrentPage)
            {
                link = new TagBuilder("span");
                item.AddCssClass("active");
            }
            else
            {
                link = new TagBuilder("a");

                string action;
                if (Selector == 0)
                {
                    action =urlHelper.Action(Action, new
                    {
                        s = Search,
                        o = CurrentProp,
                        cat = Categories,
                        desc = DescGroups,
                        c = ItemsPerPage,
                        p = pageNumber
                    });
                }
                else
                {
                    action = urlHelper.Action(Action, new
                    {
                        s = Search,
                        st = Selector,
                        o = CurrentProp,
                        cat = Categories,
                        desc = DescGroups,
                        c = ItemsPerPage,
                        p = pageNumber
                    });
                }
                link.Attributes["href"] = action;
            }
            link.AddCssClass("page-link");
            link.InnerHtml.Append(pageNumber.ToString());
            item.InnerHtml.AppendHtml(link);
            return item;
        }

    }
}
