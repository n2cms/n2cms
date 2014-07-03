using System;
using System.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
    /// <summary>
    /// A control panel plugin that is hidden unless there is a zone on the page.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ControlPanelDragPluginAttribute : ControlPanelLinkAttribute
    {
        public ControlPanelDragPluginAttribute(string name, string iconUrl, string url, string toolTip, int sortOrder, ControlPanelState showDuring)
            : base(name, iconUrl, url, toolTip, sortOrder, showDuring)
        {
            IconClass = "fa fa-th-large";
        }

        public override Control AddTo(Control container, PluginContext context)
        {
            Control control = base.AddTo(container, context);
            if(control != null)
                container.Page.PreRender += delegate { control.Visible = container.Page.Items[Zone.PageKey] != null;};
            return control;
        }
    }
}
