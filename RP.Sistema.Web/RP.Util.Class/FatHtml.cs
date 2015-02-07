using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Collections;
using System.Web.Optimization;

namespace RP.Util.Class
{
    public static class FatHtml
    {
        const string keyForBlockScript = "__key_For_Js_Block";
        const string keyForBlockStyle = "__key_For_Css_Block";

        private static MvcHtmlString AddBlock(HtmlHelper helper, Func<dynamic, System.Web.WebPages.HelperResult> template, string type)
        {
            var scriptBuilder = helper.ViewContext.HttpContext.Items[type] as StringBuilder ?? new StringBuilder();
            scriptBuilder.Append(template(null).ToHtmlString());
            helper.ViewContext.HttpContext.Items[type] = scriptBuilder;
            return new MvcHtmlString(string.Empty);
        }

        /// <summary>
        /// Adiciona um bloco JavaScript ao fim da lista de bloco de scripts
        /// </summary> 
        public static MvcHtmlString AddScriptBlock(this HtmlHelper helper, Func<dynamic, System.Web.WebPages.HelperResult> template)
        {
            return AddBlock(helper, template, keyForBlockScript);
        }

        /// <summary>
        /// Adiciona um bloco Css ao fim da lista de bloco de estilos
        /// </summary>
        public static MvcHtmlString AddStyleBlock(this HtmlHelper helper, Func<dynamic, System.Web.WebPages.HelperResult> template)
        {
            return AddBlock(helper, template, keyForBlockStyle);
        }

        /// <summary>
        /// Renderiza todos blocos de scripts
        /// </summary>
        public static MvcHtmlString RenderScriptBlocks(this HtmlHelper helper)
        {
            var scriptBuilder = helper.ViewContext.HttpContext.Items[keyForBlockScript] as StringBuilder ?? new StringBuilder();
            string content = scriptBuilder.ToString();

            if (!HttpContext.Current.IsDebuggingEnabled && !string.IsNullOrEmpty(content)) {
                string clearContent = System.Text.RegularExpressions.Regex.Replace(content, "<script[^>]*>|</script>", string.Empty);
                var minifier = new Microsoft.Ajax.Utilities.Minifier();
                content = string.Format("<script type=\"text/javascript\">{0}</script>", minifier.MinifyJavaScript(clearContent));
            }

            return new MvcHtmlString(content);
        }

        /// <summary>
        /// Renderiza todos blocos de estilos
        /// </summary>
        public static MvcHtmlString RenderStyleBlocks(this HtmlHelper helper)
        {
            var scriptBuilder = helper.ViewContext.HttpContext.Items[keyForBlockStyle] as StringBuilder ?? new StringBuilder();
            string content = scriptBuilder.ToString();

            if (!HttpContext.Current.IsDebuggingEnabled && !string.IsNullOrEmpty(content))
            {
                string clearContent = System.Text.RegularExpressions.Regex.Replace(content, "<style[^>]*>|</style>", string.Empty);
                var minifier = new Microsoft.Ajax.Utilities.Minifier();
                content = string.Format("<style type=\"text/css\">{0}</style>", minifier.MinifyStyleSheet(clearContent));
            }

            return new MvcHtmlString(content);
        }
    }
}
