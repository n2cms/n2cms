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
			div.Attributes["class"] = "sort command";
			container.Controls.Add(div);

            HtmlAnchor up = AddSortAnchor(div, context.Format("Navigation/sortUp.ashx?selected={Selected.Path}", true), "~/Edit/img/Ico/png/bullet_arrow_up.png", "up");
			RegisterToolbarUrl(container, up.ClientID, "Navigation/sortUp.ashx?selected={selected}");

            HtmlAnchor down = AddSortAnchor(div, context.Format("Navigation/sortDown.ashx?selected={Selected.Path}", true), "~/Edit/img/Ico/png/bullet_arrow_down.png", "down");
			RegisterToolbarUrl(container, down.ClientID, "Navigation/sortDown.ashx?selected={selected}");

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

			a.InnerHtml = string.Format("<img src='{0}' alt=''/>", N2.Web.Url.ToAbsolute(iconUrl));

			container.Controls.Add(a);
			return a;
		}
	}
}
