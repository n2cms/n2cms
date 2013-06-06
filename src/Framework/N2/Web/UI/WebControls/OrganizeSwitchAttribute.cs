using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit;
using System.Web.UI.WebControls;

namespace N2.Web.UI.WebControls
{
	public class OrganizeSwitchAttribute : ControlPanelLinkAttribute
	{
		public OrganizeSwitchAttribute()
			: base ("cpOrganize", "{ManagementUrl}/Resources/icons/layout_edit.png", "{Selected.Url}", "Organize parts", -10, ControlPanelState.Visible | ControlPanelState.DragDrop)
		{
			UrlEncode = false;
		}

		public override System.Web.UI.Control AddTo(System.Web.UI.Control container, PluginContext context)
		{
			HyperLink link = (HyperLink)base.AddTo(container, context);
			if (context.HttpContext.Request.QueryString["edit"] == "drag")
				link.CssClass += " toggled";
			else
			{
				link.CssClass += " complementary";
				link.NavigateUrl = link.NavigateUrl.ToUrl().AppendQuery("edit=drag");
			}
			return link;
		}
	}
}
