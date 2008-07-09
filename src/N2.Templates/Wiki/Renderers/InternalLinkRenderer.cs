using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Web.UI;
using N2.Web;

namespace N2.Templates.Wiki.Renderers
{
    public class InternalLinkRenderer : IRenderer
    {
        #region IControlRenderer Members

        public Control AddTo(Control container, ViewContext context)
        {
            string name = context.Fragment.Value.Trim('[',']');
            ContentItem existingArticle = context.Article.WikiRoot.GetChild(name);
            Url url = new Url(context.Article.WikiRoot.Url).AppendSegment(name);
            bool exists = existingArticle == null || existingArticle != context.Article.WikiRoot;
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
