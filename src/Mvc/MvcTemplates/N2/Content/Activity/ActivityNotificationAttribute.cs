using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.UI;
using N2.Engine;
using N2.Edit.Activity;
using System.Web.UI;
using N2.Web;
using N2.Edit;

namespace N2.Management.Activity
{
    public class ActivityNotificationAttribute : Attribute, IContentPageConcern
    {
        public ActivityNotificationAttribute()
        {
            Operation = "Edit";
        }

        public string Operation { get; set; }

        public void OnPreInit(System.Web.UI.Page page, ContentItem item)
        {
            var engine = page.GetEngine();
            if (item != null && engine.Config.Sections.Management.Collaboration.ActivityTrackingEnabled)
                engine.AddActivity(new ManagementActivity { Operation = Operation, PerformedBy = page.User.Identity.Name, Path = item.Path, ID = item.ID });

			var collaboration = engine.Config.Sections.Management.Collaboration;
			if (!collaboration.ActivityTrackingEnabled)
				return;

			var script = string.Format(@"
setInterval(function() {{ 
	$.get('{0}?activity={4}&{1}={2}', function(result){{ 
		try {{ 
			n2 && n2.context && n2.context(result) 
		}} catch (ex) {{ console.log(ex); }} }}).fail(function(result){{ try {{ n2 && n2.failure && n2.failure(result) }} catch (ex) {{ console.log(ex); }} }}); }}, {3});
", Url.ResolveTokens(collaboration.PingPath), PathData.SelectedQueryKey, new SelectionUtility(page, engine).SelectedItem.Path, collaboration.PingInterval * 1000, Operation);

			page.InitComplete += (s, e) => N2.Resources.Register.JavaScript(page, script, N2.Resources.ScriptOptions.DocumentReady);
        }
    }
}
