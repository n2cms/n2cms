using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Edit.Web
{
	public class EditUserControl : UserControl
	{
		protected N2.ContentItem SelectedItem
		{
			get
			{
				string itemId = Request.QueryString["item"];
				string selected = Request.QueryString["selected"];
				if (!string.IsNullOrEmpty(selected))
					return N2.Context.UrlParser.Parse(selected);
				if (!string.IsNullOrEmpty(itemId))
					return N2.Context.Persister.Get(int.Parse(itemId));
				else
					return N2.Context.UrlParser.StartPage;
			}
		}
	}
}
