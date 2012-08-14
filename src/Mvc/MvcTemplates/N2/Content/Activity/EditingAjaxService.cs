using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Web;
using N2.Engine;
using N2.Edit;
using N2.Edit.Activity;

namespace N2.Management.Activity
{
	[Service(typeof(IAjaxService))]
	public class EditingAjaxService : IAjaxService
	{
		private IEngine engine;

		public EditingAjaxService(IEngine engine)
		{
			this.engine = engine;
		}

		public string Name
		{
			get { return "editing"; }
		}

		public bool RequiresEditAccess
		{
			get { return true; }
		}

		public bool IsValidHttpMethod(string httpMethod)
		{
			return true;
		}

		public void Handle(HttpContextBase context)
		{
			var selection = new SelectionUtility(context.Request, engine);

			if (Convert.ToBoolean(context.Request["activity"]))
				engine.AddActivity(new ManagementActivity { Operation = "Edit", PerformedBy = context.User.Identity.Name, ID = selection.SelectedItem.ID, Path = selection.SelectedItem.Path });

			var activities = ManagementActivity.GetActivity(engine, selection.SelectedItem);
			context.Response.ContentType = "application/json";
			context.Response.Write(ManagementActivity.ToJson(activities));
		}
	}
}