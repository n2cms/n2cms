using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.UI;
using N2.Engine;
using N2.Edit.Activity;

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
			engine.AddActivity(new ManagementActivity { Operation = Operation, PerformedBy = page.User.Identity.Name, Path = item.Path, ID = item.ID });
		}
	}
}
