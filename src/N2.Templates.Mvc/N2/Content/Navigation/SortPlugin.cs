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

			HtmlAnchor up = AddSortAnchor(div, context.Rebase(context.Format("Navigation/sortUp.ashx?selected={Selected.Path}", true)), "~/N2/Resources/icons/bullet_arrow_up.png", "up");
			RegisterToolbarUrl(container, up.ClientID, context.Rebase("Content/Navigation/sortUp.ashx?selected={selected}"));

			HtmlAnchor down = AddSortAnchor(div, context.Rebase(context.Format("Navigation/sortDown.ashx?selected={Selected.Path}", true)), "~/N2/Resources/icons/bullet_arrow_down.png", "down");
			RegisterToolbarUrl(container, down.ClientID, context.Rebase("Content/Navigation/sortDown.ashx?selected={selected}"));

			return div;
		}

		protected virtual HtmlAnchor AddSortAnchor(Control container, string url, string iconUrl, string key)
		{
			HtmlAnchor a = new HtmlAnchor();
			a.ID = key + Name;
			a.HRef = url;

			a.Target = Target;
			a.Attributes["class"] = key;
			a.Title = Utility.GetResourceString(GlobalResourceClassName, key + Name + ".ToolTip") ?? ToolTip;
			a.Style[HtmlTextWriterStyle.BackgroundImage] = N2.Web.Url.ToAbsolute(iconUrl);

			container.Controls.Add(a);
			return a;
		}
	}
}
