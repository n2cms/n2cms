using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.Templates.Advertisement.UI.WebControls
{
	public class Bookmark : Control
	{

		public string UrlFormat
		{
			get { return (string)(ViewState["UrlFormat"] ?? string.Empty); }
			set { ViewState["UrlFormat"] = value; }
		}



		public string Image
		{
			get { return (string)(ViewState["Image"] ?? string.Empty); }
			set { ViewState["Image"] = value; }
		}

		public string Text
		{
			get { return (string)(ViewState["Text"] ?? string.Empty); }
			set { ViewState["Text"] = value; }
		}

		protected override void Render(HtmlTextWriter writer)
		{
			BookmarkList list = Parent as BookmarkList;
			RenderList(list, writer);
		}

		private void RenderList(BookmarkList list, HtmlTextWriter writer)
		{
			if (list == null) throw new ArgumentNullException("list");

			string format = list.ShowText 
				? "<a href='{0}'><img src='{1}{2}' alt='{3}'/>{3}</a>"
				: "<a href='{0}'><img src='{1}{2}' alt='{3}'/></a>";

			writer.Write(format, 
				string.Format(UrlFormat, list.BookmarkUrl, list.BookmarkText),
				VirtualPathUtility.ToAbsolute(list.ImageFolder), 
				Image, 
				Text);
		}
	}
}
