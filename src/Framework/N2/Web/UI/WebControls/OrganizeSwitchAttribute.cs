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
            IconClass = "fa fa-th-large";
            Target = Targets.Top;
			Legacy = true;
        }

        public override System.Web.UI.Control AddTo(System.Web.UI.Control container, PluginContext context)
        {
            HyperLink link = (HyperLink)base.AddTo(container, context);
            if (context.HttpContext.Request.QueryString["edit"] == "drag")
            {
                link.CssClass += " toggled";
                link.NavigateUrl = Engine.ManagementPaths.GetManagementInterfaceUrl().ToUrl().AppendSelection(context.Selected);
            }
            else
            {
                link.CssClass += " complementary";
                link.NavigateUrl = Engine.ManagementPaths.GetManagementInterfaceUrl().ToUrl().AppendQuery("mode=Organize").AppendSelection(context.Selected);
            }
            return link;
        }
    }
}
