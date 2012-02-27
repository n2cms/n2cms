using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Security;
using N2.Web;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
	/// <summary>
	/// Used internally to add the dicard preview button.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ControlPanelPreviewDiscardAttribute : ControlPanelLinkAttribute
	{
		public ControlPanelPreviewDiscardAttribute(string toolTip, int sortOrder)
            : base("cpPreviewDiscard", "{ManagementUrl}/Resources/icons/cancel.png", null, toolTip, sortOrder, ControlPanelState.Previewing)
		{
		}

		public override Control AddTo(Control container, PluginContext context)
		{
			if (!context.Engine.SecurityManager.IsAuthorized(this, context.HttpContext.User, context.Selected))
				return null;

			if(!ActiveFor(container, context.State)) return null;
			if (!context.Selected.VersionOf.HasValue) return null;

			HyperLink hl = new HyperLink();
			hl.Text = GetInnerHtml(context, IconUrl, ToolTip, Title);
			hl.NavigateUrl = Url.Parse("{ManagementUrl}/Content/DiscardPreview.aspx").ResolveTokens().AppendQuery("selectedUrl", context.Selected.Url);
			hl.CssClass = "cancel";
			hl.Attributes["onclick"] = "return confirm('Are you certain?');";

			hl.ToolTip = Utility.GetResourceString(GlobalResourceClassName, Name + ".ToolTip") ?? context.Format(ToolTip, false);

			container.Controls.Add(hl);

			return hl;
		}

		protected void RedirectTo(Page page, ContentItem item)
		{
			string url = page.Request["returnUrl"];
			if (string.IsNullOrEmpty(url))
				url = Engine.GetContentAdapter<NodeAdapter>(item).GetPreviewUrl(item);

			page.Response.Redirect(url);
		}
	}
}