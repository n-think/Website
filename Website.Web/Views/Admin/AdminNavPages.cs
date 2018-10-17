using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Website.Web.Views.Admin
{
    public static class AdminNavPages
    {
        public static string ActivePageKey => "ActivePage";

        public static string Index => "Index";

        public static string Users => "Users";

        public static string Items => "Items";

        public static string Orders => "Orders";


        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string UsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Users);

        public static string ItemsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Items);

        public static string OrdersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Orders);


        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string;
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }

        public static void AddActivePage(this ViewDataDictionary viewData, string activePage) => viewData[ActivePageKey] = activePage;
    }
}
