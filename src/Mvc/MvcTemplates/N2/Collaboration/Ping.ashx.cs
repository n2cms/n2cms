using N2.Web;
using N2.Engine;
using N2.Edit;
using N2.Edit.Activity;
using N2.Management.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Edit.Collaboration;
using N2.Management.Api;

namespace N2.Management.Collaboration
{
	/// <summary>
	/// Summary description for Notify
	/// </summary>
	public class Ping : IHttpHandler
	{
		Logger<Ping> log;
		public void ProcessRequest(HttpContext context)
		{
			try
			{
				var activity = context.Request["activity"];
				var engine = N2.Context.Current;
				if (activity == "Edit")
				{
					NotifyEditing(engine, new HttpContextWrapper(context));
				}
				else if (activity == "View")
				{
					NotifyViewing(engine, new HttpContextWrapper(context));
				}
			}
			catch (Exception ex)
			{
				log.Error(ex);
				context.Response.Status = "500 Server error";
				context.Response.ContentType = "application/json";
				context.Response.WriteJson(new { Runnnig = false });
			}
		}

		private void NotifyViewing(IEngine engine, HttpContextWrapper context)
		{
			var selection = new SelectionUtility(context, engine);
			if (selection.SelectedItem != null)
				engine.AddActivity(new ManagementActivity { Operation = "View", PerformedBy = context.User.Identity.Name, ID = selection.SelectedItem.ID, Path = selection.SelectedItem.Path });

			var cctx = CollaborationContext.Create(engine.Resolve<IProfileRepository>(), selection.SelectedItem, context);
			context.Response.WriteJson(new
			{
				Messages = engine.Resolve<ManagementMessageCollector>()
					.GetMessages(cctx)
					.ToList(),
				Flags = new FlagData(engine.Resolve<ManagementFlagCollector>().GetFlags(cctx))
			});
		}

		private void NotifyEditing(IEngine engine, HttpContextWrapper context)
		{
			var selection = new SelectionUtility(context, engine);
			if (Convert.ToBoolean(context.Request["changes"]))
				engine.AddActivity(new ManagementActivity { Operation = "Edit", PerformedBy = context.User.Identity.Name, ID = selection.SelectedItem.ID, Path = selection.SelectedItem.Path });

			var activities = ManagementActivity.GetActivity(engine, selection.SelectedItem);
			var cctx = CollaborationContext.Create(engine.Resolve<IProfileRepository>(), selection.SelectedItem, context);
			var messages = engine.Resolve<N2.Edit.Collaboration.ManagementMessageCollector>()
				.GetMessages(cctx)
				.ToList();
			var flags = engine.Resolve<ManagementFlagCollector>().GetFlags(cctx).ToList();
			context.Response.ContentType = "application/json";
			context.Response.Write(ManagementActivity.ToJson(activities, messages, flags));
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}
