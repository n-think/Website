using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Website.Service.Interfaces;

namespace Website.Web.TagHelpers
{
    [HtmlTargetElement("user-claims", TagStructure = TagStructure.WithoutEndTag)]
    public class UserClaimsTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        public UserClaimsTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public string Name { get; set; }
        public string Display { get; set; }
        public string Actions { get; set; }
        public IList<Claim> Claims { get; set; }
        public bool ViewOnly { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            var labelDiv = new TagBuilder("div");
            var label = new TagBuilder("label");
            label.AddCssClass("claims-label");
            label.InnerHtml.Append(Display);
            labelDiv.InnerHtml.AppendHtml(label);

            //Add labelDiv
            output.PostContent.AppendHtml(labelDiv);

            string type = "";
            if (Display.StartsWith("Пользователи"))
            {
                type = "users";
            }
            if (Display.StartsWith("Товары"))
            {
                type = "items";
            }
            if (Display.StartsWith("Заказы"))
            {
                type = "orders";
            }

            var reverseCheckboxHtml = new List<IHtmlContent>();
            var actionSplit = Actions.Split(" ").Reverse();
            var masterFlag = false;
            foreach (var action in actionSplit)
            {
                var checkboxDiv = new TagBuilder("div");
                var input = new TagBuilder("input");
                var checkboxLabel = new TagBuilder("label");

                var capAction = char.ToUpper(action[0]) + action.Substring(1);
                var capType = char.ToUpper(type[0]) + type.Substring(1);

                input.Attributes.Add("type", "checkbox");
                input.AddCssClass(type);
                input.AddCssClass(action);
                input.Attributes.Add("name", "newClaims");
                input.Attributes.Add("value", capAction + capType);
                checkboxLabel.AddCssClass(type);
                checkboxLabel.AddCssClass(action);

                var actionLabel = "";
                switch (action)
                {
                    case "view":
                        actionLabel = "Просмотр";
                        break;
                    case "edit":
                        actionLabel = "Редактирование";
                        break;
                    case "create":
                        actionLabel = "Создание";
                        break;
                    case "delete":
                        actionLabel = "Удаление";
                        break;
                }
                checkboxLabel.InnerHtml.Append(actionLabel);

                if (ViewOnly || masterFlag) //viewonly
                {
                    input.Attributes.Add("disabled", "");
                }

                if (masterFlag || Claims.Any(x => x.Type == capAction + capType))
                {
                    input.Attributes.Add("checked","");
                    masterFlag = true;
                }

                checkboxDiv.InnerHtml.AppendHtml(input);
                checkboxDiv.InnerHtml.AppendHtml(checkboxLabel);
                //Add checkboxDiv
                reverseCheckboxHtml.Add(checkboxDiv);
            }

            reverseCheckboxHtml.Reverse();
            foreach (var item in reverseCheckboxHtml)
            {
                output.PostContent.AppendHtml(item);
            }
        }
    }
}
