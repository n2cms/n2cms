using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.Edit.Web
{
    public class EditSiteMapNode : SiteMapNode
    {
        public EditSiteMapNode(SiteMapProvider provider, ContentItem item)
            : base(provider, item.ID.ToString())
        {
            this.CurrentItem = item;
        }

        public EditSiteMapNode(SiteMapProvider provider, string url)
			: this(provider, Engine.UrlParser.Parse(url))
        {
        }

        private ContentItem currentItem;
        public ContentItem CurrentItem
        {
            get { return currentItem; }
			set
			{
				this.currentItem = value;
				if (value != null)
				{
					this.Url = value.RewrittenUrl;
					this.Title = value.Title;
					this.Description = value.ID + ", " + value.Name
						+ " (" + value.Published + " - " + (value.Expires.HasValue ? value.Expires.ToString() : "") + ") "
						+ Engine.Definitions.GetDefinition(value.GetType()).Title;
				}
				else
				{
					this.Url = null;
					this.Title = null;
					this.Description = null;
				}
			}
        }

		protected static Engine.IEngine Engine
		{
			get { return N2.Context.Current; }
		}
    }
}
