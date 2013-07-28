using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace N2.Edit.Web.UI.Controls
{
	public class ItemLink : HyperLink
	{
		public ItemLink()
		{
			ShowIcon = true;
		}

		public object DataSource { get; set; }

		public string InterfaceUrl { get; set; }

		public bool ShowIcon { get; set; }

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
		
			var item = DataSource as ContentItem;
			if (item == null)
				return;

			if (string.IsNullOrEmpty(InterfaceUrl))
				this.NavigateUrl = N2.Context.Current.GetContentAdapter<NodeAdapter>(item).GetPreviewUrl(item);
			else
				this.NavigateUrl = InterfaceUrl.ResolveUrlTokens().ToUrl().AppendSelection(item);

			if (!ShowIcon)
			{
				Text = item.Title;
			}
			else if (string.IsNullOrEmpty(item.IconUrl))
			{
				Text = string.Format("<b class='{0}'></b> {1}", item.IconClass, item.Title);
			}
			else
			{
				Text = string.Format("<img src='{0}' alt='{1}' /> {2}", item.IconUrl.ResolveUrlTokens(), item.GetContentType().Name, string.IsNullOrEmpty(item.Title) ? "(untitled)" : item.Title);
			}
		}
	}
}