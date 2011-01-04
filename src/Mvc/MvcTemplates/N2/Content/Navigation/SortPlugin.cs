using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Edit.Navigation
{
	public class SortPluginAttribute : ToolbarPluginAttribute
	{
		public SortPluginAttribute()
			: base("", "sort", "#", ToolbarArea.Operations, "navigation", "", -40)
		{
			ToolTip = "move up";
			GlobalResourceClassName = "Toolbar";
		}

		protected override string ArrayVariableName
		{
			get { return "toolbarPlugIns"; }
		}

		public override Control AddTo(Control container, PluginContext context)
		{
			HtmlGenericControl div = new HtmlGenericControl("div");
			div.Attributes["class"] = "sort";
			container.Controls.Add(div);

			HtmlAnchor up = AddSortAnchor(div, context,
			                              context.Format("{ManagementUrl}/Navigation/sortUp.ashx?selected={Selected.Path}", true),
			                              "{ManagementUrl}/Resources/icons/bullet_arrow_up.png", "up");
			RegisterToolbarUrl(container, up.ClientID, context.Rebase("{ManagementUrl}/Content/Navigation/sortUp.ashx?selected={selected}"));

			HtmlAnchor down = AddSortAnchor(div, context,
			                                context.Format("{ManagementUrl}/Navigation/sortDown.ashx?selected={Selected.Path}", true),
			                                "{ManagementUrl}/Resources/icons/bullet_arrow_down.png", "down");
			RegisterToolbarUrl(container, down.ClientID, context.Rebase("{ManagementUrl}/Content/Navigation/sortDown.ashx?selected={selected}"));

			return div;
		}

		protected virtual HtmlAnchor AddSortAnchor(Control container, PluginContext context, string url, string iconUrl, string key)
		{
			HtmlAnchor a = new HtmlAnchor();
			a.ID = key + Name;
			a.HRef = context.Rebase(url);

			a.Target = Target;
			a.Attributes["class"] = key;
			a.Title = Utility.GetResourceString(GlobalResourceClassName, key + Name + ".ToolTip") ?? ToolTip;
			a.Style[HtmlTextWriterStyle.BackgroundImage] = context.Rebase(iconUrl);

			container.Controls.Add(a);
			return a;
		}
	}
}
