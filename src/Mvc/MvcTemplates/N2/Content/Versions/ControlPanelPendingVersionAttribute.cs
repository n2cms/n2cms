using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Engine;
using N2.Persistence;
using N2.Web;
using N2.Web.UI.WebControls;

namespace N2.Edit.Versions
{
	/// <summary>
	/// Used internally to add the dicard preview button.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ControlPanelPendingVersionAttribute : ControlPanelLinkAttribute
	{
		public ControlPanelPendingVersionAttribute(string toolTip, int sortOrder)
			: base("cpPendingVersion", "{IconsUrl}/book_next.png", null, toolTip, sortOrder, ControlPanelState.Visible)
		{
		}

		public override Control AddTo(Control container, PluginContext context)
		{
			if(!ActiveFor(container, context.State)) return null;
			if (context.Selected == null) return null;
			if (context.Selected.VersionOf.HasValue) return null;

			IEngine engine = Context.Current;
			ContentItem latestVersion = engine.Resolve<IVersionManager>().GetVersionsOf(context.Selected, 1)[0];
			if (latestVersion == context.Selected) return null;

			Url versionPreviewUrl = engine.GetContentAdapter<NodeAdapter>(latestVersion).GetPreviewUrl(latestVersion);
			versionPreviewUrl = versionPreviewUrl.AppendQuery("preview", latestVersion.ID);
			versionPreviewUrl = versionPreviewUrl.AppendQuery("original", context.Selected.ID);
			versionPreviewUrl = versionPreviewUrl.AppendQuery("returnUrl=" + engine.GetContentAdapter<NodeAdapter>(context.Selected).GetPreviewUrl(context.Selected));

			HyperLink hl = new HyperLink();
			hl.NavigateUrl = versionPreviewUrl;
			hl.Text = GetInnerHtml(context, latestVersion.State == ContentState.Waiting ? "{IconsUrl}/clock_play.png".ResolveUrlTokens() : IconUrl, ToolTip, Title);
			hl.CssClass = "preview";
			hl.ToolTip = Utility.GetResourceString(GlobalResourceClassName, Name + ".ToolTip") ?? context.Format(ToolTip, false);
			container.Controls.Add(hl);

			return hl;
		}
	}
}