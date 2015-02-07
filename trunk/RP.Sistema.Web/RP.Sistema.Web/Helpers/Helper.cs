using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Security.Cryptography;
using System.Text;
using RP.Util.Class;
using System.ServiceModel;

namespace RP.Sistema.Web.Helpers
{
    public static class Menu
    {
        public static MvcHtmlString BuildMenu(this HtmlHelper helper, List<Models.MenuVM> items, int limit)
        {
            TagBuilder li, a, ul2, li2, ul3, li3, div;
            string result = string.Empty;
            List<Models.MenuVM> menus, grupos;
            int i;

            // separa os menus por modulo
            foreach (var modulo in items.GroupBy(e => e.Modulo).Select(e => e.First()))
            {
                // cria tags para montar a raiz dos menus
                a = new TagBuilder("a");
                a.Attributes.Add("href", "#");
                a.Attributes.Add("class", "dropdown-toggle");
                a.Attributes.Add("data-toggle", "dropdown");
                a.InnerHtml = modulo.Modulo + @" <strong class=""caret""></strong>";

                li = new TagBuilder("li");
                li.AddCssClass("dropdown");
                li.InnerHtml = a.ToString();

                menus = items.Where(e => e.Modulo == modulo.Modulo).ToList();
                
                // se a quantidade de menus for menor-igual ao limite, monta na estrutura padrao
                if (menus.Count <= limit)
                {
                    ul2 = new TagBuilder("ul");
                    ul2.Attributes.Add("class", "dropdown-menu");
                    ul2.Attributes.Add("role", "menu");
                    ul2.Attributes.Add("aria-labelledby", "dLabel");

                    i = 0;
                    grupos = menus.GroupBy(e => e.Grupo).Select(e => e.First()).ToList();
                    foreach (var grupo in grupos)
                    {
                        li2 = new TagBuilder("li");
                        li2.AddCssClass("nav-header");
                        li2.SetInnerText(grupo.Grupo);

                        ul2.InnerHtml += li2.ToString();

                        foreach (var menu in menus.Where(e => e.Grupo == grupo.Grupo))
                        {
                            a = new TagBuilder("a");
                            a.Attributes.Add("href", menu.Url);
                            a.Attributes.Add("class", "open-tab draggable");
                            a.Attributes.Add("data-icon-class", menu.Icone);
                            a.SetInnerText(menu.Nome);

                            li2 = new TagBuilder("li");
                            li2.InnerHtml = a.ToString();

                            ul2.InnerHtml += li2.ToString();
                        }

                        if (++i < grupos.Count)
                        {
                            li2 = new TagBuilder("li");
                            li2.AddCssClass("divider");

                            ul2.InnerHtml += li2.ToString();
                        }
                    }

                    li.InnerHtml += ul2.ToString();
                }
                // monta o menu em estrutura diferente
                else 
                {
                    ul2 = new TagBuilder("ul");
                    ul2.Attributes.Add("class", "dropdown-menu");
                    ul2.Attributes.Add("role", "menu");
                    ul2.Attributes.Add("aria-labelledby", "dLabel");

                    grupos = menus.GroupBy(e => e.Grupo).Select(e => e.First()).ToList();
                    foreach (var grupo in grupos)
                    {
                        a = new TagBuilder("a");
                        a.Attributes.Add("href", "#");
                        a.Attributes.Add("class", "dropdown-toggle");
                        a.SetInnerText(grupo.Grupo);

                        li2 = new TagBuilder("li");
                        li2.AddCssClass("dropdown-submenu");
                        li2.InnerHtml = a.ToString();

                        IEnumerable<Models.MenuVM> allMenus = menus.Where(e => e.Grupo == grupo.Grupo);

                        if (allMenus.Count() <= 10)
                        {
                            ul3 = new TagBuilder("ul");
                            ul3.AddCssClass("dropdown-menu");
                            ul3.Attributes.Add("role", "menu");
                            ul3.Attributes.Add("aria-labelledby", "dLabel");

                            foreach (var menu in allMenus)
                            {
                                a = new TagBuilder("a");
                                a.Attributes.Add("href", menu.Url);
                                a.Attributes.Add("class", "open-tab draggable");
                                a.Attributes.Add("data-icon-class", menu.Icone);
                                a.SetInnerText(menu.Nome);

                                li3 = new TagBuilder("li");
                                li3.InnerHtml = a.ToString();
                                ul3.InnerHtml += li3.ToString();
                            }

                            li2.InnerHtml += ul3.ToString();
                        }
                        else
                        {
                            List<IEnumerable<Models.MenuVM>> listOfMenus = new List<IEnumerable<Models.MenuVM>>();
                            for (int c = 0; c < allMenus.Count(); c += 12)
                            {
                                listOfMenus.Add(allMenus.Skip(c).Take(12));
                            }

                            div = new TagBuilder("div");
                            div.AddCssClass("dropdown-menu wrap-splited");
                            div.Attributes.Add("role", "menu");
                            div.Attributes.Add("aria-labelledby", "dLabel");

                            foreach (var _menus in listOfMenus)
                            {
                                ul3 = new TagBuilder("ul");
                                ul3.AddCssClass("dropdown-menu splited-dropdown");
                                ul3.Attributes.Add("role", "menu");
                                ul3.Attributes.Add("aria-labelledby", "dLabel");

                                foreach (var menu in _menus)
                                {
                                    a = new TagBuilder("a");
                                    a.Attributes.Add("href", menu.Url);
                                    a.Attributes.Add("class", "open-tab draggable");
                                    a.Attributes.Add("data-icon-class", menu.Icone);
                                    a.SetInnerText(menu.Nome);

                                    li3 = new TagBuilder("li");
                                    li3.InnerHtml = a.ToString();
                                    ul3.InnerHtml += li3.ToString();
                                }

                                div.InnerHtml += ul3.ToString();
                            }

                            li2.InnerHtml += div.ToString();
                        }

                        ul2.InnerHtml += li2.ToString();
                    }

                    li.InnerHtml += ul2.ToString();
                }

                result += li.ToString();
            }

            return MvcHtmlString.Create(result);
        }
    }

    public static class Helper
    {
        public static readonly string modulo = "sistema";

        public static bool isAuthenticated
        {
            get { return System.Web.HttpContext.Current.User.Identity.IsAuthenticated; }
        }

        public static string UserLogin
        {
            get
            {
                return isAuthenticated ? (HttpContext.Current.User as CustomPrincipal).Login : string.Empty;
            }
        }

        public static int UserId
        {
            get
            {
                return isAuthenticated ? (HttpContext.Current.User as CustomPrincipal).Id : 0;
            }
        }
    }

    public class APIKeyAuthorization : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            //var keys = HttpContext.Current.Cache[RP.Util.Class.APIKeyAuthorizationManager.APIKEYLIST] as List<Guid>;
            //if (keys == null)
            //{
               var keys = new List<Guid> { new Guid(System.Configuration.ConfigurationManager.AppSettings["API_KEY"]) };
            //}

            return RP.Util.Class.APIKeyAuthorizationManager.CheckAccessCore(operationContext, keys);
        }
    }
}