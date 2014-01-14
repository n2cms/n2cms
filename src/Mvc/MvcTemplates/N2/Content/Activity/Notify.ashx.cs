using N2.Web;
using N2.Engine;
using N2.Edit;
using N2.Edit.Activity;
using N2.Management.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Content.Activity
{
    /// <summary>
    /// Summary description for Notify
    /// </summary>
    public class Notify : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
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

        private void NotifyViewing(IEngine engine, HttpContextWrapper context)
        {
            var selection = new SelectionUtility(context, engine);
            if (selection.SelectedItem != null)
                engine.AddActivity(new ManagementActivity { Operation = "View", PerformedBy = context.User.Identity.Name, ID = selection.SelectedItem.ID, Path = selection.SelectedItem.Path });
        }

        private void NotifyEditing(IEngine engine, HttpContextWrapper context)
        {
            var selection = new SelectionUtility(context, engine);
            if (Convert.ToBoolean(context.Request["changes"]))
                engine.AddActivity(new ManagementActivity { Operation = "Edit", PerformedBy = context.User.Identity.Name, ID = selection.SelectedItem.ID, Path = selection.SelectedItem.Path });

            var activities = ManagementActivity.GetActivity(engine, selection.SelectedItem);
            context.Response.ContentType = "application/json";
            context.Response.Write(ManagementActivity.ToJson(activities));
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
