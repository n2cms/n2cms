using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace N2.Edit.Navigation
{
	public class SortPluginAttribute : ToolbarPluginAttribute
	{
		public SortPluginAttribute()
			: base("", "sortUp", "#", ToolbarArea.Navigation, "navigation", "", -40)
		{
			ToolTip = "move up";
			GlobalResourceClassName = "Toolbar";
		}

		protected override string ArrayVariableName
		{
			get { return "toolbarPlugIns"; }
		}

		public override Control AddTo(Control container)
		{
			HtmlGenericControl div = new HtmlGenericControl("div");
			div.Attributes["class"] = "sort command";
			container.Controls.Add(div);

			HtmlAnchor up = AddSortAnchor(div, "Navigation/sortUp.ashx?selected={selected}", "~/Edit/img/Ico/bullet_arrow_up.gif", "up");
			RegisterToolbarUrl(container, up.ClientID, "Navigation/sortUp.ashx?selected={selected}");

			HtmlAnchor down = AddSortAnchor(div, "Navigation/sortDown.ashx?selected={selected}", "~/Edit/img/Ico/bullet_arrow_down.gif", "down");
			RegisterToolbarUrl(container, down.ClientID, "Navigation/sortDown.ashx?selected={selected}");

			return div;
		}

		protected virtual HtmlAnchor AddSortAnchor(Control container, string urlFormat, string iconUrl, string key)
		{
			HtmlAnchor a = new HtmlAnchor();
			a.ID = key + Name;
			a.HRef = urlFormat
				.Replace("~/", Utility.ToAbsolute("~/"))
				.Replace("{selected}", "")
				.Replace("{memory}", "")
				.Replace("{action}", "");

			a.Target = Target;
			a.Attributes["class"] = key;
			a.Title = Utility.GetResourceString(GlobalResourceClassName, key + Name + ".ToolTip") ?? ToolTip;

			a.InnerHtml = string.Format("<img src='{0}' alt=''/>", Utility.ToAbsolute(iconUrl));

			container.Controls.Add(a);
			return a;
		}
	}
}
