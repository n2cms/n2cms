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
			Legacy = true;
        }

        public override System.Web.UI.Control AddTo(System.Web.UI.Control container, PluginContext context)
        {
            if (context.Selected == null)
                return null;

			var collaboration = context.Engine.Config.Sections.Management.Collaboration;
			if (!collaboration.ActivityTrackingEnabled)
                return null;

            var script = new LiteralControl(string.Format(@"<script>
setInterval(function() {{ $.get('{0}?activity=View&{1}={2}', function(result){{ try {{ n2 && n2.context && n2.context(result) }} catch (ex) {{ console.log(ex); }} }}).fail(function(result){{ try {{ n2 && n2.failure && n2.failure(result) }} catch (ex) {{ console.log(ex); }} }}); }}, {3});
</script>", Url.ResolveTokens(collaboration.PingPath), PathData.SelectedQueryKey, context.Selected.Path, collaboration.PingInterval * 1000));
            container.Controls.Add(script);
            return script;
        }
    }
}
