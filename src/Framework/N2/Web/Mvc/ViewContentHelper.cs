using System;
using System.Web.Mvc;
using N2.Web.Mvc.Html;
using N2.Engine;

namespace N2.Web.Mvc
{
    /// <summary>
    /// Provides quick acccess to often used APIs.
    /// </summary>
    public class ViewContentHelper : ContentHelperBase
    {
        public ViewContentHelper(HtmlHelper html)
            : base(html.ContentEngine, html.CurrentPath)
        {
            Html = html;
        }

        public ViewContentHelper(HtmlHelper html, Func<IEngine> engineGetter, Func<PathData> pathGetter)
            : base(engineGetter, pathGetter)
        {
            Html = html;
        }

        public HtmlHelper Html { get; private set; }

        public virtual RegisterHelper Register
        {
            get { return new RegisterHelper(Html); }
        }

        public virtual RenderHelper Render
        {
            get { return new RenderHelper { Html = Html, Content = Current.Item }; }
        }

        public bool HasValue(string detailName)
        {
            return Current.Item != null && Current.Item[detailName] != null && !(string.Empty.Equals(Current.Item[detailName]));
        }
    }


}
