#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Web.Parts;
using N2.Web.Mvc;
using N2.Engine;
using System.Web.Mvc;
using N2.Web.UI;
using N2.Web.Mvc.Html;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Templates.Mvc.Areas.Tests
{
    public interface IWebFormPart
    {
    }

    [Adapts(typeof(IWebFormPart))]
    public class WebFormOnMvcPartAdapter : PartsAdapter
    {
        public override void RenderPart(HtmlHelper html, ContentItem part, System.IO.TextWriter writer = null)
        {
            ContentPage page = new ContentPage();
            page.CurrentPage = html.CurrentPage();

            Engine.ResolveAdapter<PartsAdapter>(page.CurrentPage).AddChildPart(part, page);

            page.RenderControl(new HtmlTextWriter(html.ViewContext.Writer));
        }
    }
}
#endif
