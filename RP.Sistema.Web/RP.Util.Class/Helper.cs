using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq.Expressions;
using System.Web;
using System.Collections.Specialized;
using RP.Util.Class.Grid;
using System.Web.Security;

namespace RP.Util.Class
{
    namespace Grid
    {
        public struct teGridColumn<T>
        {
            public string header { get; set; }
            public IDictionary<string, string> columnAttributes { get; set; }
            public IDictionary<string, string> headerAttributes { get; set; }
            public Func<T, object> format { get; set; }
        }

        public struct teGridPager
        {
            public int pageSize { get; set; }
            public int totalItemCount { get; set; }
            public int currentPage { get; set; }
            public string controller { get; set; }
            public string action { get; set; }
            public System.Web.Routing.RouteValueDictionary routeValues { get; set; }
            public IDictionary<string, string> pagerAttributes { get; set; }
            public string messageNoRecord { get; set; }
        }

        public static class Helper
        {
            public static MvcHtmlString tGrid<T>(this HtmlHelper helper, IEnumerable<T> source, IDictionary<string, string> gridAttributes, IEnumerable<teGridColumn<T>> columns, IDictionary<string, string> alternateAttributes = null, IDictionary<string, string> rowAttributes = null, teGridPager? pager = null, string rowTotal = "")
            {
                // verifica se nao existe registro, se nao existir, retorna mensagem
                if (source == null || source.Count() == 0)
                {
                    string message = "<p>Nenhum registro encontrado</p>";

                    if (pager != null && pager.Value.totalItemCount == 0)
                    {
                        if (!string.IsNullOrEmpty(pager.Value.messageNoRecord))
                        {
                            message = pager.Value.messageNoRecord;
                        }
                    }
                    return MvcHtmlString.Create(message);
                }

                string result, tpl;
                MvcHtmlString pagination;
                TagBuilder table = new TagBuilder("table");

                tpl = @"<div class=""datatable"">{0}</div>";

                foreach (var item in gridAttributes)
                {
                    table.Attributes.Add(item);
                }

                if (source != null)
                {
                    if (source.Count() > 0)
                    {
                        /*
                         *Inicio da geração do header
                         */

                        TagBuilder thead = new TagBuilder("thead");

                        TagBuilder tr = new TagBuilder("tr");
                        TagBuilder td = null;

                        int countCol = 0;

                        foreach (var col in columns)
                        {
                            td = new TagBuilder("th");

                            if (col.headerAttributes != null)
                            {
                                foreach (var item in col.headerAttributes)
                                {
                                    td.Attributes.Add(item);
                                }
                            }

                            td.InnerHtml = col.header;

                            tr.InnerHtml += td.ToString();

                            countCol++;
                        }

                        thead.InnerHtml = tr.ToString();
                        table.InnerHtml += thead.ToString();

                        /*
                         *Final da geração do header
                         */


                       /*
                        *Inicio da geração do body
                        */
                        int rowNumber = 0;

                        TagBuilder tbody = new TagBuilder("tbody");

                        if (source != null)
                        {
                            foreach (T data in source)
                            {
                                rowNumber++;

                                tr = new TagBuilder("tr");

                                /*
                                 * 
                                 */
                                if (rowNumber % 2 == 0)
                                {
                                    if (alternateAttributes != null)
                                    {
                                        foreach (var item in alternateAttributes)
                                        {
                                            tr.Attributes.Add(item);
                                        }
                                    }
                                }
                                else
                                {
                                    if (rowAttributes != null)
                                    {
                                        foreach (var item in rowAttributes)
                                        {
                                            tr.Attributes.Add(item);
                                        }
                                    }
                                }

                                foreach (var col in columns)
                                {
                                    td = new TagBuilder("td");

                                    if (col.columnAttributes != null)
                                    {
                                        foreach (var item in col.columnAttributes)
                                        {
                                            td.Attributes.Add(item);
                                        }
                                    }


                                    if (col.format != null)
                                    {
                                        td.InnerHtml = col.format(data) == null ? "" : col.format(data).ToString();
                                    }

                                    tr.InnerHtml += td.ToString();
                                }

                                tbody.InnerHtml += tr.ToString();
                            }
                            if (!string.IsNullOrEmpty(rowTotal))
                            {
                                tbody.InnerHtml += rowTotal;
                            }
                        }
                        else
                        {
                            tr = new TagBuilder("tr");
                            td = new TagBuilder("td");
                            td.Attributes.Add("colspan", countCol.ToString());
                            td.InnerHtml = "&nbsp;";

                            tr.InnerHtml += td.ToString();

                            tbody.InnerHtml += tr.ToString();
                        }
                        table.InnerHtml += tbody.ToString();


                        /*
                        *Final da geração do body
                        */
                    }
                }


                pagination = RenderPager(helper, pager);

                result = RenderTopGrid(pager, pagination).ToString();
                result += table.ToString();
                result += RenderBottomGrid(pager, pagination).ToString();

                return MvcHtmlString.Create(string.Format(tpl, result));
            }



            private static MvcHtmlString RenderTopGrid(teGridPager? pagerParam, MvcHtmlString pager)
            {
                TagBuilder select, option;
                string tpl;

                if (pagerParam != null && pagerParam.Value.totalItemCount > 0 && pagerParam.Value.totalItemCount > int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE))
                {
                    tpl = @"<div class=""row-fluid""><div class=""span6""><div class=""datatable-length""><label> {0} registros por página</label></div></div><div class=""span6""><div class=""datatable-pagination pagination pagination-right"">{1}</div></div></div>";

                    select = new TagBuilder("select");
                    select.AddCssClass("datatable-pagesize");

                    if (!select.Attributes.ContainsKey("title"))
                    {
                        select.Attributes.Add("title", "Quantidade de registros por página");
                    }

                    for (int i = 10; i <= 100 && i <= pagerParam.Value.totalItemCount + 9; i += 10)
                    {
                        option = new TagBuilder("option") { 
                            InnerHtml = i.ToString() 
                        };

                        if (i == pagerParam.Value.pageSize)
                        {
                            option.MergeAttribute("selected", "selected");
                        }

                        option.MergeAttribute("value", i.ToString());
                        select.InnerHtml = String.Format("{0}\n{1}", select.InnerHtml, option.ToString());
                    }

                    return MvcHtmlString.Create(string.Format(tpl, select.ToString(), pager));
                }

                return MvcHtmlString.Create(null);
            }

            private static MvcHtmlString RenderBottomGrid(teGridPager? pagerParam, MvcHtmlString pager)
            {
                string tpl, info = string.Empty;
                if (pagerParam != null && pagerParam.Value.totalItemCount > 0 && pagerParam.Value.totalItemCount > int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE))
                {
                    tpl = @"<div class=""row-fluid""><div class=""span4""><div class=""datatable-info"">{0}</div></div><div class=""span6""><div class=""datatable-pagination pagination pagination-right"">{1}</div></div></div>";
                    if (pagerParam != null)
                    {
                        if (pagerParam.Value.totalItemCount == 1)
                        {
                            info = "001 registro encontrado";
                        }
                        else
                        {
                            info = pagerParam.Value.totalItemCount.ToString("000") + " registros encontrados";
                        }
                    }
                    return MvcHtmlString.Create(string.Format(tpl, info, pager));
                }
                return MvcHtmlString.Create(null);
            }

            private static MvcHtmlString RenderPager(this HtmlHelper helper, teGridPager? pagerParam)
            {
                string result = string.Empty;
                if (pagerParam != null)
                {
                    if (pagerParam.Value.totalItemCount > 0 && pagerParam.Value.totalItemCount > int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE))
                    {
                        result = "<ul>";

                        var pageCount = (int)Math.Ceiling(pagerParam.Value.totalItemCount / (double)pagerParam.Value.pageSize);
                        const int nrOfPagesToDisplay = 6;
                        var sb = new System.Text.StringBuilder();

                        // First
                        //sb.Append(pagerParam.currentPage == 1 ? @"<li class=""disabled""><span>«</span></li>" : ("<li>" + GeneratePageLink("«", "Primeira página", 1, pagerParam.pageSize, helper.ViewContext.RequestContext, pagerParam.routeValues) + "</li>"));

                        // Previous
                        sb.Append(pagerParam.Value.currentPage > 1 ? ("<li>" + GeneratePageLink("Anterior", "Página anterior", pagerParam.Value.currentPage - 1, pagerParam.Value.pageSize, helper.ViewContext.RequestContext, pagerParam.Value.routeValues) + "</li>") : @"<li class=""disabled""><span>Anterior</span></li>");

                        var start = 1;
                        var end = pageCount;
                        if (pageCount > (nrOfPagesToDisplay + 3))
                        {
                            var middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
                            var below = (pagerParam.Value.currentPage - middle);
                            var above = (pagerParam.Value.currentPage + middle);
                            if (below < 4)
                            {
                                above = nrOfPagesToDisplay;
                                below = 1;
                            }
                            else if (above > (pageCount - 4))
                            {
                                above = pageCount;
                                below = (pageCount - nrOfPagesToDisplay);
                            }
                            start = below;
                            end = above;
                        }
                        if (start > 2)
                        {
                            sb.Append("<li>" + GeneratePageLink("1", 1, pagerParam.Value.pageSize, helper.ViewContext.RequestContext, pagerParam.Value.routeValues) + "</li>");
                            sb.Append("<li>" + GeneratePageLink("2", 2, pagerParam.Value.pageSize, helper.ViewContext.RequestContext, pagerParam.Value.routeValues) + "</li>");
                            sb.Append(@"<li class=""disabled""><span>...</span></li>");
                        }
                        for (var i = start; i <= end; i++)
                        {
                            if (i == pagerParam.Value.currentPage || (pagerParam.Value.currentPage <= 0 && i == 0))
                            {
                                sb.AppendFormat(@"<li class=""active""><a>{0}</a></li>", i);
                            }
                            else
                            {
                                sb.Append("<li>" + GeneratePageLink(i.ToString(), i, pagerParam.Value.pageSize, helper.ViewContext.RequestContext, pagerParam.Value.routeValues) + "</li>");
                            }
                        }
                        if (end < (pageCount - 3))
                        {
                            sb.Append(@"<li class=""disabled""><span>...</span></li>");
                            sb.Append("<li>" + GeneratePageLink((pageCount - 1).ToString(), pageCount - 1, pagerParam.Value.pageSize, helper.ViewContext.RequestContext, pagerParam.Value.routeValues) + "</li>");
                            sb.Append("<li>" + GeneratePageLink(pageCount.ToString(), pageCount, pagerParam.Value.pageSize, helper.ViewContext.RequestContext, pagerParam.Value.routeValues) + "</li>");
                        }

                        // Next
                        sb.Append(pagerParam.Value.currentPage < pageCount ? ("<li>" + GeneratePageLink("Próximo", "Próxima página", (pagerParam.Value.currentPage + 1), pagerParam.Value.pageSize, helper.ViewContext.RequestContext, pagerParam.Value.routeValues) + "</li>") : @"<li class=""disabled""><span>Próximo</span></li>");

                        // Last
                        //sb.Append(pagerParam.currentPage == pageCount ? @"<li class=""disabled""><span>»</span></li>" : ("<li>" + GeneratePageLink("»", "Última página", pageCount, pagerParam.pageSize, helper.ViewContext.RequestContext, pagerParam.routeValues) + "</li>"));

                        result += sb.ToString();
                        result += "</ul>";
                    }
                }
                return MvcHtmlString.Create(result);
            }

            private static string GeneratePageLink(string linkText, int pageNumber, int pageSize, RequestContext context, RouteValueDictionary routeValues)
            {
                return _GeneratePageLink(linkText, string.Empty, pageNumber, pageSize, context, routeValues);
            }

            private static string GeneratePageLink(string linkText, string titleText, int pageNumber, int pageSize, RequestContext context, RouteValueDictionary routeValues)
            {
                return _GeneratePageLink(linkText, titleText, pageNumber, pageSize, context, routeValues);
            }

            private static string _GeneratePageLink(string linkText, string titleText, int pageNumber, int pageSize, RequestContext context, RouteValueDictionary routeValues)
            {
                routeValues.Remove("page");
                routeValues.Remove("pagesize");
                var pageLinkValueDictionary = new RouteValueDictionary(routeValues) { { "page", pageNumber }, { "pagesize", pageSize } };
                var virtualPathForArea = RouteTable.Routes.GetVirtualPathForArea(context, pageLinkValueDictionary);

                routeValues = new RouteValueDictionary();
                foreach (string key in context.HttpContext.Request.QueryString.AllKeys)
                {
                    //value = context.HttpContext.Request.QueryString[key];
                    if (!routeValues.ContainsKey(key) && key != "page" && key != "pagesize")
                    {
                        //foreach (string value in context.HttpContext.Request.QueryString[key].Split(','))
                        //{
                        string[] value = context.HttpContext.Request.QueryString[key].Split(',');
                        routeValues.Add(key, value);
                        //}
                    }
                }

                foreach (string key in routeValues.Keys)
                {
                    foreach (string value in ((string[])routeValues[key]))
                    {
                        virtualPathForArea.VirtualPath += "&" + key + "=" + value;
                    }
                }

                //if (virtualPathForArea == null)				
                //    return null; 
                var stringBuilder = new System.Text.StringBuilder("<a");
                //if (ajaxOptions != null)				
                //    foreach (var ajaxOption in ajaxOptions.ToUnobtrusiveHtmlAttributes())					
                //        stringBuilder.AppendFormat(" {0}=\"{1}\"", ajaxOption.Key, ajaxOption.Value); 
                stringBuilder.AppendFormat(" href=\"{0}\" title=\"{1}\">{2}</a>", virtualPathForArea.VirtualPath, titleText, linkText);
                return stringBuilder.ToString();
            }

        }
      
    }

    public static class Helper
    {
        private static string ToQueryString(NameValueCollection nvc)
        {
            return "?" + string.Join("&", Array.ConvertAll(nvc.AllKeys, key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
        }

        public static int UserId
        {
            get
            {
                HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

                if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(ticket.UserData);
                    return obj.Id;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static string UserLogin
        {
            get 
            {
                HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

                if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(ticket.UserData);
                    return obj.Login;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public static SelectListItem[] DiaSemana()
        {
            return new[] {
                new SelectListItem { Text = "Segunda", Value = "Segunda" }, 
                new SelectListItem { Text = "Terça", Value = "Terça" }, 
                new SelectListItem { Text = "Quarta", Value = "Quarta" }, 
                new SelectListItem { Text = "Quinta", Value = "Quinta" }, 
                new SelectListItem { Text = "Sexta", Value = "Sexta" }, 
                new SelectListItem { Text = "Sábado", Value = "Sábado" }, 
                new SelectListItem { Text = "Domingo", Value = "Domingo" }
            };
        }

        public static readonly SelectListItem[] TipoAnotacao = new[]
        {
            new SelectListItem { Text = "Medição", Value = "Medição" },
            new SelectListItem { Text = "Início", Value = "Início" }, 
            new SelectListItem { Text = "Encerramento", Value = "Encerramento" }
        };

        public static readonly SelectListItem[] SituacaoLeito = new[]
        {
            new SelectListItem { Text = "Ocupado", Value = "Ocupado" },
            new SelectListItem { Text = "Livre", Value = "Livre" }, 
            new SelectListItem { Text = "Manutenção", Value = "Manutenção" }
        };

        public static readonly SelectListItem[] TipoFormaPagamento = new[]
        {
            new SelectListItem { Text = "A vista", Value = "A vista" }, 
            new SelectListItem { Text = "A prazo", Value = "A prazo" }
        };

        public static readonly SelectListItem[] TipoAprovacao = new[]
        {
            new SelectListItem { Text = "", Value = "" }, 
            new SelectListItem { Text = "Aprovar", Value = "Aprovada" }, 
            new SelectListItem { Text = "Negar", Value = "Negada" }
        };
        public static readonly SelectListItem[] TipoReceitaDespesa = new[]
        {
            new SelectListItem { Text = "", Value = "" }, 
            new SelectListItem { Text = "Receita", Value = "Receita" }, 
            new SelectListItem { Text = "Despesa", Value = "Despesa" }
        };

        public enum eSimNao
        { 
            Sim,
            Não
        }

        public static SelectListItem[] SimNao(eSimNao? defaultValue = null)
        {
            return new[] {
                new SelectListItem { Text = "Sim", Value = "Sim",  Selected=(defaultValue == eSimNao.Sim) }, 
                new SelectListItem { Text = "Não", Value = "Não" , Selected=(defaultValue == eSimNao.Não)}
            };
        }

        public static Form Editor(this HtmlHelper helper)
        {
            Form _form = new Form();

            return _form;
        }
        
        public static MvcHtmlString BindParam(this HtmlHelper helper, NameValueCollection querystring, string notBind)
        {
            string hiddens = string.Empty;
            List<string> k = new List<string>(notBind.Split(',').Select(s => s.Trim()));
            k = k.Distinct().ToList();
            foreach (string key in querystring)
            {
                if (!k.Contains(key.Trim()))
                {
                    hiddens += "<input type='hidden' name='" + key + "' value='" + querystring[key] + "' />"; 
                }
            }
            return MvcHtmlString.Create(hiddens);
        }

        public static string getInputName(string name, string prefixo)
        {
            return (string.IsNullOrEmpty(prefixo) ? "" : prefixo + ".") + name;
        }

        public static string getInputId(string id, string prefixo)
        {
            return ((string.IsNullOrEmpty(prefixo) ? "" : prefixo + "_") + id).Replace('.', '_');
        }

        public static object KeepFilter(this Controller controller, string filter = null)
        {
            if (string.IsNullOrEmpty(filter))
            {
                object result = controller.Session[controller.ToString() + "filter"];
                if (result != null)
                {
                    return controller.Session[controller.ToString() + "filter"].ToString();
                }
            }
            else
            {
                controller.Session[controller.ToString() + "filter"] = filter;
                return null;
            }

            return null;
        }
    }

    public static partial class HtmlExtensions
    {
        public static MvcHtmlString ClientPrefixName(this HtmlHelper htmlHelper)
        {
            return MvcHtmlString.Create(htmlHelper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix.Replace('.', '_'));
        }

        public static MvcHtmlString ClientNameFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return MvcHtmlString.Create(htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression)));
        }

        public static MvcHtmlString ClientIdFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return MvcHtmlString.Create(htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression)));
        }
    }

    public static class Extensions
    {

        public static string logParse(this int? value)
        {
            if (value == null)
                return string.Empty;
            return value.Value.ToString();
        }

        public static string logParse(this DateTime? value, string format)
        {
            if (value == null)
                return string.Empty;
            return value.Value.ToString(format);
        }
    }

    public static class ViewExtensions
    {
        public static MvcHtmlString CustomValidationSummary(this HtmlHelper html, bool closeable = true, bool hideProperties = true, string validationMessage = "", object htmlAttributes = null)
        {
            if (!html.ViewData.ModelState.IsValid)
            {

                TagBuilder div = new TagBuilder("div");
                string properties = string.Empty;

                // adiciona os atributos
                if (htmlAttributes != null)
                {
                    var type = htmlAttributes.GetType();
                    var props = type.GetProperties();

                    foreach (var item in props)
                    {
                        div.MergeAttribute(item.Name, item.GetValue(htmlAttributes, null).ToString());
                    }
                }

                if (closeable)
                { 
                    div.InnerHtml += @"<button type=""button"" class=""close"" data-dismiss=""alert"">×</button>";
                    div.AddCssClass("fade in");
                }

                // adiciona mensagem na div
                div.InnerHtml += validationMessage;

                if (!hideProperties)
                {
                    foreach (var key in html.ViewData.ModelState.Keys)
                    {
                        foreach (var err in html.ViewData.ModelState[key].Errors)
                        {
                            properties += "<p>" + html.Encode(err.ErrorMessage) + "</p>";
                        }
                    }

                    if (!string.IsNullOrEmpty(properties))
                    {
                        div.InnerHtml += properties;
                    }
                }

                return MvcHtmlString.Create(div.ToString());
            }

            return null;
        }

        public static MvcHtmlString CustomActionLink(this HtmlHelper html, string linkText, string actionName, object htmlAttributes, object icons = null, bool hideText = false)
        {
            return CustomActionLink(html, linkText, actionName, null, new { }, htmlAttributes, icons, hideText);
        }

        public static MvcHtmlString CustomActionLink(this HtmlHelper html, string linkText, string actionName, object routeValues, object htmlAttributes, object icons = null, bool hideText = false)
        {
            return CustomActionLink(html, linkText, actionName, null, routeValues, htmlAttributes, icons, hideText);
        }

        public static MvcHtmlString CustomActionLink(this HtmlHelper html, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes, object icons = null, bool hideText = false)
        {
            UrlHelper urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            string  iconLeft = string.Empty, 
                    iconRight = string.Empty, 
                    innerHtml = string.Empty;

            TagBuilder a = new TagBuilder("a");
            TagBuilder i;
            bool hasTitle = false;

            if (string.IsNullOrEmpty(controllerName))
            {
                a.Attributes.Add("href", actionName.StartsWith("#") ? actionName : urlHelper.Action(actionName, routeValues));
            }
            else
            {
                a.Attributes.Add("href", actionName.StartsWith("#") ? actionName : urlHelper.Action(actionName, controllerName, routeValues));
            }
            

            // adiciona os atributos
            if (htmlAttributes != null)
            {
                var type = htmlAttributes.GetType();
                var props = type.GetProperties();
                foreach (var item in props)
                {
                    if (!hasTitle)
                    {
                        hasTitle = item.Name.ToLower().Equals("title");
                    }

                    a.MergeAttribute(item.Name, item.GetValue(htmlAttributes, null).ToString());
                }
            }

            // adiciona os icones
            if (icons != null)
            {
                var type = icons.GetType();
                var props = type.GetProperties();
                foreach (var item in props)
                {
                    if (item.Name.ToLower().Equals("left"))
                    {
                        iconLeft = item.GetValue(icons, null).ToString();
                    }
                    else if (item.Name.ToLower().Equals("right"))
                    {
                        iconRight = item.GetValue(icons, null).ToString();
                    }
                }
            }

            if (!string.IsNullOrEmpty(iconLeft))
            {
                i = new TagBuilder("i");
                i.AddCssClass(iconLeft);
                innerHtml += i.ToString() + " ";
            }

            if (!hideText)
            {
                innerHtml += linkText;
            }

            if (!hasTitle && hideText)
            {
                a.Attributes.Add("title", linkText);
            }

            if (!string.IsNullOrEmpty(iconRight))
            {
                i = new TagBuilder("i");
                i.AddCssClass(iconRight);
                innerHtml += " " + i.ToString();
            }

            a.InnerHtml = innerHtml;

            return MvcHtmlString.Create(a.ToString());
        }

        public static MvcHtmlString CustomButton(this HtmlHelper html, string buttonText, object htmlAttributes, object icons = null, bool hideText = false)
        {
            return CustomButton(html, buttonText, null, htmlAttributes, icons, hideText);
        }

        public static MvcHtmlString CustomButton(this HtmlHelper html, string buttonText, string buttonType, object htmlAttributes, object icons = null, bool hideText = false)
        {
            UrlHelper urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            string iconLeft = string.Empty,
                    iconRight = string.Empty,
                    innerHtml = string.Empty;

            TagBuilder button = new TagBuilder("button");
            TagBuilder i;
            bool hasTitle = false;

            if (string.IsNullOrEmpty(buttonType))
            {
                button.Attributes.Add("type", "button");
            }
            else
            {
                button.Attributes.Add("type", buttonType);
            }


            // adiciona os atributos
            if (htmlAttributes != null)
            {
                var type = htmlAttributes.GetType();
                var props = type.GetProperties();
                foreach (var item in props)
                {
                    if (!hasTitle)
                    {
                        hasTitle = item.Name.ToLower().Equals("title");
                    }

                    button.MergeAttribute(item.Name, item.GetValue(htmlAttributes, null).ToString());
                }
            }

            // adiciona os icones
            if (icons != null)
            {
                var type = icons.GetType();
                var props = type.GetProperties();
                foreach (var item in props)
                {
                    if (item.Name.ToLower().Equals("left"))
                    {
                        iconLeft = item.GetValue(icons, null).ToString();
                    }
                    else if (item.Name.ToLower().Equals("right"))
                    {
                        iconRight = item.GetValue(icons, null).ToString();
                    }
                }
            }

            if (!string.IsNullOrEmpty(iconLeft))
            {
                i = new TagBuilder("i");
                i.AddCssClass(iconLeft);
                innerHtml += i.ToString() + " ";
            }

            if (!hideText)
            {
                innerHtml += buttonText;
            }

            if (!hasTitle && hideText)
            {
                button.Attributes.Add("title", buttonText);
            }

            if (!string.IsNullOrEmpty(iconRight))
            {
                i = new TagBuilder("i");
                i.AddCssClass(iconRight);
                innerHtml += " " + i.ToString();
            }

            button.InnerHtml = innerHtml;

            return MvcHtmlString.Create(button.ToString());
        }

        public static MvcHtmlString CustomLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            return CustomLabelFor(html, expression, string.Empty, htmlAttributes);
        }

        public static MvcHtmlString CustomLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText, object htmlAttributes = null)
        {
            TagBuilder lbl = new TagBuilder("label");
            var metadata = ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, html.ViewData);
            
            if (string.IsNullOrEmpty(labelText))
            {
                labelText = metadata.DisplayName;
            }

            lbl.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression)));
            lbl.SetInnerText(labelText);

            // adiciona os atributos
            if (htmlAttributes != null)
            {
                var type = htmlAttributes.GetType();
                var props = type.GetProperties();
                foreach (var item in props)
                {
                    lbl.MergeAttribute(item.Name, item.GetValue(htmlAttributes, null).ToString());
                }
            }

            return MvcHtmlString.Create(lbl.ToString());
        }

    }

}
