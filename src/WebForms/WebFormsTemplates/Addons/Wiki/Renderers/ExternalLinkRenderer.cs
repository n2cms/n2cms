using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Addons.Wiki.Renderers
{
    public class ExternalLinkRenderer : IRenderer
    {
        #region IRenderer Members

        public Control AddTo(Control container, ViewContext context)
        {
            string[] link = context.Fragment.ToString().Trim('[', ']').Split('|');
            HtmlAnchor a = new HtmlAnchor();
            a.HRef = link[0];
            a.InnerHtml = link.Length > 1 ? link[1] : link[0];
            a.Attributes["rel"] = "nofollow"; // fight linkspam
            container.Controls.Add(a);
            return a;
        }

        #endregion
    }
}
