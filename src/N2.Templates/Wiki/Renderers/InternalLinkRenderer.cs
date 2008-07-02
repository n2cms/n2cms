using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace N2.Templates.Wiki.Renderers
{
    public class InternalLinkRenderer : IRenderer
    {
        #region IControlRenderer Members

        public Control AddTo(Control container, RenderingContext context)
        {
            string name = context.Fragment.Value.Trim('[',']');
            ContentItem existingArticle = context.Wiki.GetChild(name);
            string url = context.Wiki.Url.Insert(context.Wiki.Url.Length - ContentItem.DefaultExtension.Length, "/" + name);
            bool exists = existingArticle == null || existingArticle != context.Wiki;
            HtmlAnchor a = new HtmlAnchor();
            a.HRef = url;
            a.InnerHtml = name;
            a.Attributes["class"] = exists ? null : "new";
            container.Controls.Add(a);
            return a;
        }

        #endregion
    }
}
