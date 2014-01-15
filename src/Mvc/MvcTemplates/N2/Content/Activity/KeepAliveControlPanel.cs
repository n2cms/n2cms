using N2.Edit;
using N2.Web;
using N2.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace N2.Management.Content.Activity
{
    public class KeepAliveControlPanelAttribute : ControlPanelLinkAttribute
    {
        public KeepAliveControlPanelAttribute()
            : base("KeepAlive", null, null, null, 10000, ControlPanelState.Visible)
        {
        }

        public override System.Web.UI.Control AddTo(System.Web.UI.Control container, PluginContext context)
        {
            if (context.Selected == null)
                return null;

            if (!context.Engine.Config.Sections.Management.Collaboration.ActivityTrackingEnabled)
                return null;

            var script = new LiteralControl(string.Format(@"<script>
setInterval(function() {{ $.get('{0}?activity=View&{1}={2}'); }}, 60000);
</script>", Url.ResolveTokens("{ManagementUrl}/Content/Activity/Notify.ashx"), PathData.SelectedQueryKey, context.Selected.Path));
            container.Controls.Add(script);
            return script;
        }
    }
}
