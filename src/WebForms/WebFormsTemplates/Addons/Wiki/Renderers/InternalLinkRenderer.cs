using System;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using N2.Web;
using System.IO;
using N2.Engine;
using System.Web.UI.WebControls;

namespace N2.Addons.Wiki.Renderers
{
    public class InternalLinkRenderer : IRenderer
    {
        IWebContext webContext;
        public InternalLinkRenderer(IWebContext webContext)
        {
            this.webContext = webContext;
        }

        #region IControlRenderer Members

        public Control AddTo(Control container, ViewContext context)
        {
            string fragment = context.Fragment.ToString().Trim('[',']');
            int colonIndex = fragment.IndexOf(':');
            if (colonIndex >= 0)
            {
                string type = fragment.Substring(0, colonIndex);
                fragment = fragment.Substring(colonIndex + 1);
                if(type.Equals("image", StringComparison.InvariantCultureIgnoreCase))
                {
                    return CreateImage(container, context, fragment);
                }
            }
            return CreateInternalLink(container, context, fragment);
        }

        /// <summary>Testability seam. Do not change. Invokes System.IO.File.Exists.</summary>
        public static Func<string, bool> FileExists = File.Exists;

        private Control CreateImage(Control container, ViewContext context, string fragment)
        {
            string[] fragments = fragment.Split('|');

            string uploadUrl = context.Article.WikiRoot.UploadFolder;
            if (string.IsNullOrEmpty(uploadUrl))
            {
                return AppendWarning(container, "The wiki's upload path is not specified.");
            }

            string uploadPath = webContext.MapPath(uploadUrl);
            string filePath = Path.Combine(uploadPath, fragments[0]);

            string name = fragments[0].Trim();
            if (FileExists(filePath))
            {
                return AppendImage(container, Url.Parse(uploadUrl).AppendSegment(name), fragments.Length > 1 ? fragments[1] : fragments[0], context.Article.WikiRoot.ImageWidth);
            }
            
            string url = Url.Parse(context.Article.WikiRoot.Url).AppendSegment("Upload").AppendQuery("parameter", fragments[0].Trim()).AppendQuery("returnUrl", context.Article.Url);
            return AppendAnchor(container, name, url, false);
        }

        private static Control AppendImage(Control container, string src, string alt, int width)
        {
            HyperLink a = new HyperLink();
            a.NavigateUrl = src;
            container.Controls.Add(a);

            Image img = new Image();
            img.AlternateText = alt;
            img.ImageUrl = src;
            if (width > 0) img.Width = width;
            a.Controls.Add(img);

            return a;
        }

        protected Control AppendWarning(Control container, string p)
        {
            HtmlGenericControl span = new HtmlGenericControl("span");
            span.Attributes["class"] = "warning";
            span.InnerHtml = p;
            container.Controls.Add(span);
            return span;
        }

        private Control CreateInternalLink(Control container, ViewContext context, string fragment)
        {
            string[] fragments = fragment.Split('|');
            ContentItem existingArticle = context.Article.WikiRoot.GetChild(fragments[0]);
            if (existingArticle != null && !existingArticle.Equals(context.Article.WikiRoot))
            {
                return AppendAnchor(container, fragments.Length > 1 ? fragments[1] : fragments[0], existingArticle.Url, true);
            }
            else
            {
                string url = Url.Parse(context.Article.WikiRoot.Url).AppendSegment(fragments[0]);
                return AppendAnchor(container, fragments.Length > 1 ? fragments[1] : fragments[0], url, false);
            }
        }

        private static Control AppendAnchor(Control container, string name, string url, bool exists)
        {
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
