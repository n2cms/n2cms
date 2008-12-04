using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ControlPanelPreviewDiscardAttribute : ControlPanelLinkAttribute
	{
		public ControlPanelPreviewDiscardAttribute(string toolTip, int sortOrder)
			: base("cpCancel", "~/edit/img/ico/cancel.gif", null, toolTip, sortOrder, ControlPanelState.Previewing)
		{
		}

		public override Control AddTo(Control container, PluginContext context)
		{
			if(!ActiveFor(container, context.State))
				return null;

			LinkButton btn = new LinkButton();
			btn.Text = GetInnerHtml(IconUrl, ToolTip, Title);
			btn.CssClass = "cancel";
			btn.OnClientClick = "return confirm('Are you certain?');";
			container.Controls.Add(btn);
			
			btn.Command += delegate
				{
					ContentItem previewedItem = Context.Current.Persister.Get(int.Parse(container.Page.Request["preview"])); ;
					if (previewedItem.VersionOf == null) throw new N2Exception("Cannot publish item that is not a version of another item");
					ContentItem published = previewedItem.VersionOf;
					Context.Current.Persister.Delete(previewedItem);
					RedirectTo(container.Page, published);
				};
			return btn;
		}

		protected void RedirectTo(Page page, ContentItem item)
		{
			string url = page.Request["returnUrl"];
			if (string.IsNullOrEmpty(url))
				url = Context.Current.EditManager.GetPreviewUrl(item);

			page.Response.Redirect(url);
		}
	}
}