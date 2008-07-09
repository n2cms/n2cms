using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Templates.Wiki.Renderers
{
    public class ExternalLinkRenderer : IRenderer
    {
        #region IRenderer Members

        public Control AddTo(Control container, ViewContext context)
        {
            string link = context.Fragment.Value.Trim('[', ']');
            HtmlAnchor a = new HtmlAnchor();
            a.HRef = link;
            a.InnerHtml = link;
            container.Controls.Add(a);
            return a;
        }

        #endregion
    }
}
