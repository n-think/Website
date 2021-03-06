﻿using System;
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
        public static string Categories => "Categories";
        public static string Orders => "Orders";
        public static string DescGroups => "DescGroups";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);
        public static string UsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Users);
        public static string ItemsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Items);
        public static string CategoriesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Categories);
        public static string OrdersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Orders);
        public static string DescGroupsNavClass(ViewContext viewContext) => PageNavClass(viewContext, DescGroups);

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string;
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }

        public static void AddActivePage(this ViewDataDictionary viewData, string activePage) => viewData[ActivePageKey] = activePage;
    }
}
