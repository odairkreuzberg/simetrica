using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace RP.Util.Class
{
    public class Line
    {
        TagBuilder p;

        public Line()
        {
            p = new TagBuilder("div");
            p.AddCssClass("control-group");
        }

        public Line(object htmlAttributes)
        {
            p = new TagBuilder("div");
            p.AddCssClass("control-group");

            if (htmlAttributes != null)
            {
                var type = htmlAttributes.GetType();
                var props = type.GetProperties();
                foreach (var item in props)
                {
                    p.MergeAttribute(item.Name, item.GetValue(htmlAttributes, null).ToString());
                }
            }
        }

        public Line Add(MvcHtmlString label, MvcHtmlString input, bool show = true)
        {
            string html = "{0}<div class='controls controls-row'>{1}</div>";
            string lbl;
            if (show)
            {
                if (MvcHtmlString.IsNullOrEmpty(label))
                {
                    lbl = "<label class='control-label'>&nbsp;</label>";
                }
                else
                {
                    lbl = label.ToString().Replace("<label ", "<label class='control-label' ");
                }
                html = string.Format(html, lbl, input.ToString());

                //TagBuilder l = new TagBuilder("div");
                //l.AddCssClass("control-group");

                //l.InnerHtml += MvcHtmlString.IsNullOrEmpty(label) ? "<label>&nbsp;</label>" : label.ToString();
                //l.InnerHtml += input.ToString();

                p.InnerHtml += html;
            }

            return this;
        }

        public Line Add(MvcHtmlString label, string input, bool show = true)
        {
            return Add(label, MvcHtmlString.Create(input), show);
        }

        public Line Add(MvcHtmlString label, Func<dynamic, System.Web.WebPages.HelperResult> input, bool show = true)
        {
            return Add(label, MvcHtmlString.Create(input(null).ToHtmlString()), show);
        }

        public Line Add(string label, MvcHtmlString input, bool show = true)
        {
            return Add(MvcHtmlString.Create(label), input, show);
        }

        public Line Add(string label, string input, bool show = true)
        {
            return Add(MvcHtmlString.Create(label), MvcHtmlString.Create(input), show);
        }

        public Line Add(Func<dynamic, System.Web.WebPages.HelperResult> label, Func<dynamic, System.Web.WebPages.HelperResult> input, bool show = true)
        {
            return Add(MvcHtmlString.Create(label(null).ToHtmlString()), MvcHtmlString.Create(input(null).ToHtmlString()), show);
        }

        public Line Add(Func<dynamic, System.Web.WebPages.HelperResult> label, string input, bool show = true)
        {
            return Add(MvcHtmlString.Create(label(null).ToHtmlString()), MvcHtmlString.Create(input), show);
        }

        public Line Add(Func<dynamic, System.Web.WebPages.HelperResult> label, MvcHtmlString input, bool show = true)
        {
            return Add(MvcHtmlString.Create(label(null).ToHtmlString()), input, show);
        }

        public Line Add(string label, Func<dynamic, System.Web.WebPages.HelperResult> input, bool show = true)
        {
            return Add(MvcHtmlString.Create(label), MvcHtmlString.Create(input(null).ToHtmlString()), show);
        }

        public TagBuilder get
        {
            get
            {
                return p;
            }
        }
    }

    public class Form
    {
        private TagBuilder d;

        public Form()
        {
            d = new TagBuilder("div");
        }

        internal TagBuilder html { get { return d; } set { d.InnerHtml += value; } }

        public void Clear()
        {
            d.InnerHtml = string.Empty;
        }

        public MvcHtmlString Render()
        {
            return MvcHtmlString.Create(d.InnerHtml.ToString());
        }

        public List<string> Add(Line line)
        {
            html.InnerHtml += line.get;

            return null;
        }
    }
}
